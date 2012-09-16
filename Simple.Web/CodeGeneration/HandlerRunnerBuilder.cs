using Simple.Web.Behaviors;

namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Behaviors.Implementations;
    using Helpers;
    using Http;

    /// <summary>
    /// Builds methods to run handlers for an <see cref="IContext"/>.
    /// </summary>
    internal class HandlerRunnerBuilder
    {
        private readonly Type _type;
        private readonly string _httpMethod;
        private readonly IMethodLookup _methodLookup;
        private readonly List<Expression> _blocks = new List<Expression>(); 
        private readonly LabelTarget _end = Expression.Label("end");
        private readonly ParameterExpression _handlerParameter;
        private ParameterExpression _context;
        private readonly ParameterExpression _handler;
        private ParameterExpression _status;
        private ParameterExpression _task;

        public HandlerRunnerBuilder(Type type, string httpMethod, IMethodLookup methodLookup = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
            _httpMethod = httpMethod;
            _methodLookup = methodLookup ?? new MethodLookup();
            _handlerParameter = Expression.Parameter(typeof(object), "obj");
            _handler = Expression.Variable(_type, "handler");
        }

        public Action<object, IContext> BuildRunner()
        {
            _status = Expression.Variable(typeof(Status), "status");
            _context = Expression.Parameter(typeof(IContext), "context");

            _blocks.Add(Expression.Assign(_handler, Expression.Convert(_handlerParameter, _type)));
            CreateSetupBlocks();
            _blocks.Add(BuildRunBlock());
            CreateResponseBlocks();

            CreateDisposeBlock();

            _blocks.Add(Expression.Label(_end));

            var block = Expression.Block(new[] {_handler, _status}, _blocks);

            return Expression.Lambda<Action<object, IContext>>(block, _handlerParameter, _context).Compile();
        }

        public AsyncRunner BuildAsyncRunner()
        {
            _task = Expression.Variable(typeof (Task<Status>), "task");
            _context = Expression.Parameter(typeof(IContext), "context");
            _blocks.Add(Expression.Assign(_handler, Expression.Convert(_handlerParameter, _type)));
            CreateSetupBlocks();
            _blocks.Add(BuildAsyncRunBlock());
            _blocks.Add(Expression.Label(_end));
            _blocks.Add(_task);
            var block = Expression.Block(new[] {_handler, _task}, _blocks);

            var start =
                Expression.Lambda<Func<object, IContext, Task<Status>>>(block, _handlerParameter, _context).Compile();

            _blocks.Clear();
            _context = Expression.Parameter(typeof(IContext), "context");
            _blocks.Add(Expression.Assign(_handler, Expression.Convert(_handlerParameter, _type)));
            _status = Expression.Parameter(typeof(Status), "status");

            CreateResponseBlocks();
            CreateDisposeBlock();

            _blocks.Add(Expression.Label(_end));
            block = Expression.Block(new[] {_handler}, _blocks);

            var end = Expression.Lambda<Action<object, IContext, Status>>(block, _handlerParameter, _context, _status).Compile();

            return new AsyncRunner(start, end);
        }

        private void CreateSetupBlocks()
        {
            _blocks.AddRange(CookiePropertySetter.GetCookiePropertySetters(_type, _handler, _context));

            foreach (var behaviorInfo in RequestBehaviorInfo.GetInPriorityOrder())
            {
                AddBehaviorBlock(behaviorInfo);
            }
        }

        private void CreateResponseBlocks()
        {
            CreateWriteStatusBlock();
            _blocks.AddRange(PropertyCookieSetter.GetPropertyCookieSetters(_type, _handler, _context));

            var redirectBehaviorInfo = new ResponseBehaviorInfo(typeof (object), typeof (Redirect2), Priority.High);
            AddBehaviorBlock(redirectBehaviorInfo);

            foreach (var behaviorInfo in ResponseBehaviorInfo.GetInPriorityOrder())
            {
                AddBehaviorBlock(behaviorInfo);
            }

            // Don't write response for HEAD requests
            if ("HEAD".Equals(_httpMethod, StringComparison.OrdinalIgnoreCase)) return;

            foreach (var behaviorInfo in OutputBehaviorInfo.GetInPriorityOrder())
            {
                // Only one Output block is going to be relevant
                if (AddBehaviorBlock(behaviorInfo)) return;
            }

            // If we didn't trigger an Output, try writing a View.
            _blocks.Add(Expression.Call(_methodLookup.WriteView, _handler, _context));
        }

        private bool AddBehaviorBlock(BehaviorInfo behaviorInfo)
        {
            if (behaviorInfo.BehaviorType.IsGenericType)
            {
                var genericInterface = _type.GetInterface(behaviorInfo.BehaviorType.FullName);
                if (genericInterface != null)
                {
                    _blocks.Add(BuildBehaviorBlock(behaviorInfo, genericInterface.GetGenericArguments()));
                    return true;
                }
            }
            if (behaviorInfo.BehaviorType.IsAssignableFrom(_type))
            {
                _blocks.Add(BuildBehaviorBlock(behaviorInfo));
                return true;
            }

            return false;
        }

        private Expression BuildBehaviorBlock(BehaviorInfo behaviorInfo)
        {
            var method = behaviorInfo.GetMethod();

            var methodCallExpression = BuildMethodCallExpression(method);

            // If the implementation method returns a boolean, then when it is false, stop processing.
            if (method.ReturnType == typeof(bool))
            {
                return Expression.IfThen(Expression.Not(methodCallExpression), Expression.Return(_end));
            }
            return methodCallExpression;
        }

        private MethodCallExpression BuildMethodCallExpression(MethodInfo method)
        {
            var methodCallExpression = method.GetParameters().Length == 2
                                           ? Expression.Call(method, _handler, _context)
                                           : Expression.Call(method, _handler, _context, _status);
            return methodCallExpression;
        }

        private Expression BuildBehaviorBlock(BehaviorInfo behaviorInfo, Type[] genericArguments)
        {
            var method = behaviorInfo.GetMethod(genericArguments);
            var methodCallExpression = BuildMethodCallExpression(method);

            // If the implementation method returns a boolean, then when it is false, stop processing.
            if (method.ReturnType == typeof(bool))
            {
                return Expression.IfThen(Expression.Not(methodCallExpression), Expression.Return(_end));
            }
            return methodCallExpression;
        }

        private void CreateWriteStatusBlock()
        {
            _blocks.Add(BuildWriteStatus());
        }

        private void CreateDisposeBlock()
        {
            if (typeof(IDisposable).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_handler, typeof(IDisposable).GetMethod("Dispose")));
            }
        }

        private Expression BuildWriteStatus()
        {
            return Expression.Call(_methodLookup.WriteStatusCode, _status, _context);
        }

        private Expression BuildRunBlock()
        {
            var methodInterface = _type.GetInterfaces().Single(HttpMethodAttribute.IsAppliedTo);
            var httpMethodAttribute = HttpMethodAttribute.Get(methodInterface);
            var run = _type.GetMethod(httpMethodAttribute.Method);
            var parameters = run.GetParameters();
            if (parameters.Length == 0)
            {
                return Expression.Assign(_status, Expression.Call(_handler, run));
            }

            var genericType = methodInterface.GetGenericArguments().Single();
            var getInput = typeof (GetInput).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericType);
            return Expression.Assign(_status, Expression.Call(_handler, run, Expression.Call(getInput, _context)));
        }

        private Expression BuildAsyncRunBlock()
        {
            var run = GetRunMethod();
            var parameters = run.GetParameters();
            if (parameters.Length == 0)
            {
                return Expression.Assign(_task, Expression.Call(_handler, run));
            }
            var getInput = typeof (GetInput).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[0].ParameterType);
            return Expression.Assign(_task, Expression.Call(_handler, run, Expression.Call(getInput, _context)));
        }

        private MethodInfo GetRunMethod()
        {
            var httpMethodAttribute = HttpMethodAttribute.Get(_type.GetInterfaces().Single(HttpMethodAttribute.IsAppliedTo));
            var method = _type.GetMethod(httpMethodAttribute.Method);
            return method;
        }
    }
}