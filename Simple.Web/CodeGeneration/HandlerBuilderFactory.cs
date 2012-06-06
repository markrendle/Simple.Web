namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class HandlerBuilderFactory
    {
        private readonly IConfiguration _configuration;

        public HandlerBuilderFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Func<IDictionary<string, string>, object> BuildHandlerBuilder(Type type)
        {
            var container = Expression.Constant(_configuration.Container);
            var getMethod = Expression.Call(container, _configuration.Container.GetType().GetMethod("Get").MakeGenericMethod(type));
            var instance = Expression.Variable(type);
            var construct = Expression.Assign(instance, getMethod);
            var variables = Expression.Parameter(typeof(IDictionary<string, string>));

            var block = PropertySetterBuilder.MakePropertySetterBlock(type, variables, instance, construct);

            return Expression.Lambda<Func<IDictionary<string, string>, object>>(block, variables).Compile();
        }
    }
}