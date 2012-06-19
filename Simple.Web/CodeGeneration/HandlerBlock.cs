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

            var call = Expression.Call(handler, _method);

            return Expression.Lambda(call, handler, context).Compile();
        }
        
        public Delegate GenerateAsync()
        {
            var context = Expression.Parameter(typeof(IContext));
            var handler = Expression.Parameter(_handlerType);

            var call = Expression.Call(handler, _method);

            return Expression.Lambda(call, handler, context).Compile();
        }
    }
}