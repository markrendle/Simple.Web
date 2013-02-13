using System.Collections.Concurrent;
using Simple.Web.Behaviors.Implementations;

namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Hosting;
    using Http;

    internal class PipelineFunctionFactory
    {
        private static readonly IDictionary<Type, IDictionary<string, Func<IContext, HandlerInfo, Task>>> RunMethodCache = 
                                    new Dictionary<Type, IDictionary<string, Func<IContext, HandlerInfo, Task>>>();

        private readonly Type _handlerType;
        private readonly ParameterExpression _context;
        private readonly ParameterExpression _scopedHandler;
        private readonly ParameterExpression _handler;
        private readonly ParameterExpression _handlerInfoVariable;

        public static Func<IContext, HandlerInfo, Task> Get(Type handlerType, string httpMethod)
        {
            if (!RunMethodCache.ContainsKey(handlerType) || !RunMethodCache[handlerType].ContainsKey(httpMethod))
            {
                lock (RunMethodCache)
                {
                    if (!RunMethodCache.ContainsKey(handlerType))
                    {
                        RunMethodCache.Add(handlerType, new Dictionary<string, Func<IContext, HandlerInfo, Task>>());
                    }

                    if (!RunMethodCache[handlerType].ContainsKey(httpMethod))
                    {
                        RunMethodCache[handlerType].Add(httpMethod, new PipelineFunctionFactory(handlerType).BuildAsyncRunMethod(httpMethod));
                    }
                }
            }

            return RunMethodCache[handlerType][httpMethod];
        }

        public PipelineFunctionFactory(Type handlerType)
        {
            _handlerType = handlerType;
            _context = Expression.Parameter(typeof (IContext));
            _scopedHandler = Expression.Variable(typeof (IScopedHandler));
            _handler = Expression.Variable(_handlerType);
            _handlerInfoVariable = Expression.Variable(typeof(HandlerInfo));
        }

        /// <summary>
        /// Generates a compiled method to run a Handler.
        /// </summary>
        /// <returns>A compiled delegate to run the Handler asynchronously.</returns>
        public Func<IContext, HandlerInfo, Task> BuildAsyncRunMethod(string httpMethod)
        {
            var blocks = new List<object>();

            blocks.AddRange(CreateBlocks(GetSetupBehaviorInfos()));
            var setCookieProperties = CookiePropertySetter.GetCookiePropertySetters(_handlerType, _handler, _context);
            blocks.AddRange(setCookieProperties);

            var second = new HandlerBlock(_handlerType, GetRunMethod(httpMethod));
            blocks.Add(second);
            var setPropertyCookies = PropertyCookieSetter.GetPropertyCookieSetters(_handlerType, _handler, _context);
            blocks.AddRange(setPropertyCookies);

            var redirectBehavior = new ResponseBehaviorInfo(typeof (object), typeof (Redirect2), Priority.High) { Universal = true };

            blocks.AddRange(CreateBlocks(GetResponseBehaviorInfos(redirectBehavior)));

            var outputs = GetOutputBehaviorInfos().ToList();
            if (outputs.Count > 0)
            {
                blocks.AddRange(CreateBlocks(outputs));
            }
            else
            {
                var writeViewBlock = new PipelineBlock();
                writeViewBlock.Add(typeof(WriteView).GetMethod("Impl", BindingFlags.Static | BindingFlags.Public));
                blocks.Add(writeViewBlock);
            }

            var call = BuildCallExpression(blocks);


            var createHandler = BuildCreateHandlerExpression();

            var lambdaBlock = Expression.Block(new[] { _handler }, new[] { createHandler, call });

            var lambda = Expression.Lambda(lambdaBlock, _context, _handlerInfoVariable);
            return (Func<IContext, HandlerInfo, Task>) lambda.Compile();
        }

        private Expression BuildCreateHandlerExpression()
        {
            var factory = Expression.Constant(HandlerFactory.Instance);
            var createScopedHandler = Expression.Assign(_scopedHandler, Expression.Call(factory, HandlerFactory.GetHandlerMethod, _handlerInfoVariable));
            var assignHandler = Expression.Assign(_handler, Expression.Convert(Expression.Property(_scopedHandler, "Handler"), _handlerType));
            return Expression.Block(new[] {_scopedHandler},
                                    new Expression[] {createScopedHandler, assignHandler});
        }

        private static IEnumerable<PipelineBlock> CreateBlocks(IEnumerable<BehaviorInfo> behaviorInfos)
        {
            var pipelineBlock = new PipelineBlock();
            foreach (var behaviorInfo in behaviorInfos)
            {
                var method = behaviorInfo.GetMethod();
                pipelineBlock.Add(method);
                if (method.ReturnType != typeof(void))
                {
                    yield return pipelineBlock;
                    pipelineBlock = new PipelineBlock();
                }
            }

            if (pipelineBlock.Any)
            {
                yield return pipelineBlock;
            }
        }

        private Expression BuildCallExpression(IEnumerable<object> blocks)
        {
            Expression call = Expression.Call(AsyncPipeline.DefaultStartMethod);

            foreach (var block in blocks)
            {
                if (call == null)
                {
                    var method = ((PipelineBlock)block).Generate(_handlerType);
                    call = Expression.Call(AsyncPipeline.StartMethod(_handlerType), Expression.Constant(method), _context, _handler);
                }
                else if (block is HandlerBlock)
                {
                    call = BuildCallHandlerExpression(block, call);
                }
                else
                {
                    call = Expression.Call(AsyncPipeline.ContinueWithAsyncBlockMethod(_handlerType), call,
                                           Expression.Constant(((PipelineBlock)block).Generate(_handlerType)),
                                           _context, _handler);
                }
            }
            return call;
        }

        private Expression BuildCallHandlerExpression(object block, Expression call)
        {
            var handlerBlock = (HandlerBlock) block;
            if (handlerBlock.IsAsync)
            {
                call = Expression.Call(AsyncPipeline.ContinueWithAsyncHandlerMethod(_handlerType), call,
                                       Expression.Constant(handlerBlock.GenerateAsync()),
                                       _context, _handler);
            }
            else
            {
                var runMethod = handlerBlock.Generate();
                call = Expression.Call(AsyncPipeline.ContinueWithHandlerMethod(_handlerType), call,
                                       Expression.Constant(runMethod),
                                       _context, _handler);
            }
            return call;
        }

        private MethodInfo GetRunMethod(string httpMethod)
        {
            var httpMethodAttribute = _handlerType.GetInterfaces()
                .Where(HttpMethodAttribute.IsAppliedTo)
                .Select(HttpMethodAttribute.Get)
                .Single(a => a.HttpMethod == httpMethod);

            return _handlerType.GetMethod(httpMethodAttribute.Method);
        }

        private IEnumerable<BehaviorInfo> GetSetupBehaviorInfos()
        {
            return RequestBehaviorInfo.GetInPriorityOrder().Where(HandlerHasBehavior);
        }

        private IEnumerable<BehaviorInfo> GetResponseBehaviorInfos(params ResponseBehaviorInfo[] defaults)
        {
            return ResponseBehaviorInfo.GetInPriorityOrder(defaults).Where(b => b.Universal || HandlerHasBehavior(b));
        }

        private IEnumerable<BehaviorInfo> GetOutputBehaviorInfos()
        {
            return OutputBehaviorInfo.GetInPriorityOrder().Where(HandlerHasBehavior);
        }

        private bool HandlerHasBehavior(BehaviorInfo behaviorInfo)
        {
            if (behaviorInfo.BehaviorType.IsGenericType)
            {
                if (_handlerType.GetInterface(behaviorInfo.BehaviorType.FullName) != null)
                {
                    return true;
                }
            }
            return behaviorInfo.BehaviorType.IsAssignableFrom(_handlerType);
        }
    }
}