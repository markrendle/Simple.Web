using System.Collections;
using System.Collections.Concurrent;
using System.Web.Handlers;
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
        private static readonly ICollection RunMethodCacheCollection;

        static PipelineFunctionFactory()
        {
            var cache = new Dictionary<Type, IDictionary<string, Func<IContext, HandlerInfo, Task>>>();
            RunMethodCache = cache;
            RunMethodCacheCollection = cache;
        }

        private readonly Type _handlerType;
        private readonly ParameterExpression _context;
        private readonly ParameterExpression _scopedHandler;
        private readonly ParameterExpression _handler;
        private readonly ParameterExpression _handlerInfoVariable;

        public static Func<IContext, HandlerInfo, Task> Get(Type handlerType, string httpMethod)
        {
            IDictionary<string, Func<IContext, HandlerInfo, Task>> handlerCache;
            if (!RunMethodCache.TryGetValue(handlerType, out handlerCache))
            {
                lock (RunMethodCacheCollection.SyncRoot)
                {
                    if (!RunMethodCache.TryGetValue(handlerType, out handlerCache))
                    {
                        var asyncRunMethod = new PipelineFunctionFactory(handlerType).BuildAsyncRunMethod(httpMethod);
                        handlerCache = new Dictionary<string, Func<IContext, HandlerInfo, Task>>
                            {
                                {httpMethod, asyncRunMethod}
                            };
                        RunMethodCache.Add(handlerType, handlerCache);
                        return asyncRunMethod;
                    }
                }
            }

            // It's not really worth all the locking palaver here,
            // worst case scenario the AsyncRunMethod gets built more than once.
            Func<IContext, HandlerInfo, Task> method;
            if (!handlerCache.TryGetValue(httpMethod, out method))
            {
                method = new PipelineFunctionFactory(handlerType).BuildAsyncRunMethod(httpMethod);
                handlerCache[httpMethod] = method;
            }
            return method;
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

            var buildAction = GetType().GetMethod("BuildAction", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(_handlerType);

            var setCookieProperties = CookiePropertySetter.GetCookiePropertySetters(_handlerType, _handler, _context).ToList();
            if (setCookieProperties.Any())
            {
                var setCookieDelegate = buildAction.Invoke(this, new object[] {setCookieProperties});
                blocks.Add(setCookieDelegate);
            }

            var second = new HandlerBlock(_handlerType, GetRunMethod(httpMethod));
            blocks.Add(second);

            var setPropertyCookies = PropertyCookieSetter.GetPropertyCookieSetters(_handlerType, _handler, _context).ToList();
            if (setPropertyCookies.Any())
            {
                var setPropertyCookiesDelegate = buildAction.Invoke(this, new object[] {setPropertyCookies});
                blocks.Add(setPropertyCookiesDelegate);
            }

            var setPropertyHeaders = PropertyHeaderSetter.GetPropertyHeaderSetters(_handlerType, _handler, _context).ToList();
            if (setPropertyHeaders.Any())
            {
                var setPropertyHeadersDelegate = buildAction.Invoke(this, new object[] {setPropertyHeaders});
                blocks.Add(setPropertyHeadersDelegate);
            }

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

            if (typeof(IDisposable).IsAssignableFrom(_handlerType))
            {
                var disposeBlock = new PipelineBlock();
                disposeBlock.Add(typeof(Disposable).GetMethod("Impl", BindingFlags.Static | BindingFlags.Public));
                blocks.Add(disposeBlock);
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
            HandlerBlock handlerBlock;
            PipelineBlock pipelineBlock;

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
                else if ((pipelineBlock = block as PipelineBlock) != null)
                {
                    call = Expression.Call(AsyncPipeline.ContinueWithAsyncBlockMethod(_handlerType), call,
                                           Expression.Constant((pipelineBlock).Generate(_handlerType)),
                                           _context, _handler);
                }
                else
                {
                    call = Expression.Call(AsyncPipeline.ContinueWithActionMethod(_handlerType), call,
                        Expression.Constant(block), _context, _handler);
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
            return _handlerType.GetInterfaces()
                               .Where(HttpMethodAttribute.IsAppliedTo)
                               .Select(t => HttpMethodAttribute.GetMethod(t, httpMethod))
                               .Single(a => a != null);
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

        private Action<THandler, IContext> BuildAction<THandler>(IEnumerable<Expression> blocks)
        {
            return Expression.Lambda<Action<THandler, IContext>>(Expression.Block(blocks), _handler, _context).Compile();
        }
    }
}