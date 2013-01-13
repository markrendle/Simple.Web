using Simple.Web.Behaviors.Implementations;

namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Http;

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
            get { return _method.ReturnType == typeof (Task<Status>); }
        }

        public Delegate Generate()
        {
            var context = Expression.Parameter(typeof(IContext));
            var handler = Expression.Parameter(_handlerType);

            var parameters = _method.GetParameters();
            if (parameters.Length == 0)
            {
                var call = Expression.Call(handler, _method);
                return Expression.Lambda(call, handler, context).Compile();
            }
            else if (parameters.Length == 1)
            {
                var getInput =
                    typeof (GetInput).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(parameters[0].ParameterType);
                var call = Expression.Call(handler, _method, Expression.Call(getInput, context));
                return Expression.Lambda(call, handler, context).Compile();
            }
            else
            {
                throw new InvalidOperationException("Handler methods may only take 0 or 1 parameters.");
            }
        }
        
        public Delegate GenerateAsync()
        {
            var context = Expression.Parameter(typeof(IContext));
            var handler = Expression.Parameter(_handlerType);

            var parameters = _method.GetParameters();
            if (parameters.Length == 0)
            {
                var call = Expression.Call(handler, _method);
                return Expression.Lambda(call, handler, context).Compile();
            }
            if (parameters.Length == 1)
            {
                var getInput =
                    typeof(GetInput).GetMethod("Impl", BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(parameters[0].ParameterType);
                var call = Expression.Call(handler, _method, Expression.Call(getInput, context));
                return Expression.Lambda(call, handler, context).Compile();
            }
            throw new InvalidOperationException("Handler methods may only take 0 or 1 parameters.");
        }
    }
}