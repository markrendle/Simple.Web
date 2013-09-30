namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using Simple.Web.Behaviors.Implementations;
    using Simple.Web.Http;

    internal class HandlerBlock
    {
        private readonly Type _handlerType;
        private readonly MethodInfo _method;

        public HandlerBlock(Type handlerType, MethodInfo method)
        {
            _handlerType = handlerType;
            _method = method;
        }

        public bool IsAsync
        {
            get { return _method.ReturnType == typeof(Task<Status>); }
        }

        public Delegate Generate()
        {
            var context = Expression.Parameter(typeof(IContext));
            var handler = Expression.Parameter(_handlerType);
            var checkRunException = typeof(CheckRunException).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static);
            var exception = Expression.Parameter(typeof(Exception));

            var parameters = _method.GetParameters();
            if (parameters.Length == 0)
            {
                Expression call = Expression.Call(handler, _method);
                call = Expression.TryCatch(call, Expression.Catch(exception, Expression.Call(checkRunException, exception, context)));
                return Expression.Lambda(call, handler, context).Compile();
            }
            if (parameters.Length == 1)
            {
                var getInput =
                    typeof(GetInput).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static)
                                    .MakeGenericMethod(parameters[0].ParameterType);
                Expression call = Expression.Call(handler, _method, Expression.Call(getInput, context));
                call = Expression.TryCatch(call, Expression.Catch(exception, Expression.Call(checkRunException, exception, context)));
                return Expression.Lambda(call, handler, context).Compile();
            }
            throw new InvalidOperationException("Handler methods may only take 0 or 1 parameters.");
        }

        public Delegate GenerateAsync()
        {
            var context = Expression.Parameter(typeof(IContext));
            var handler = Expression.Parameter(_handlerType);
            var checkRunExceptionAsyncCheck = typeof(CheckRunException).GetMethod("ImplAsyncCheck",
                                                                                  BindingFlags.Public | BindingFlags.Static);
            var checkRunExceptionAsync = typeof(CheckRunException).GetMethod("ImplAsync", BindingFlags.Public | BindingFlags.Static);
            var exception = Expression.Parameter(typeof(Exception));

            var parameters = _method.GetParameters();
            if (parameters.Length == 0)
            {
                Expression call = Expression.Call(checkRunExceptionAsync, Expression.Call(handler, _method), context);
                call = Expression.TryCatch(call,
                                           Expression.Catch(exception, Expression.Call(checkRunExceptionAsyncCheck, exception, context)));
                return Expression.Lambda(call, handler, context).Compile();
            }
            if (parameters.Length == 1)
            {
                var getInput =
                    typeof(GetInput).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static)
                                    .MakeGenericMethod(parameters[0].ParameterType);
                Expression call = Expression.Call(checkRunExceptionAsync,
                                                  Expression.Call(handler, _method, Expression.Call(getInput, context)),
                                                  context);
                call = Expression.TryCatch(call,
                                           Expression.Catch(exception, Expression.Call(checkRunExceptionAsyncCheck, exception, context)));
                return Expression.Lambda(call, handler, context).Compile();
            }
            throw new InvalidOperationException("Handler methods may only take 0 or 1 parameters.");
        }
    }
}