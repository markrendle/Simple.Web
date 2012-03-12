namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web;

    class EndpointFactory
    {
        private static EndpointFactory _instance;
        public static EndpointFactory Instance
        {
            get { return _instance ?? (_instance = new EndpointFactory()); }
        }

        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, IEndpoint>> _getBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, IEndpoint>>();
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, IEndpoint>> _postBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, IEndpoint>>();

        public IEndpoint GetEndpoint(Type type, IDictionary<string,string> variables)
        {
            var builder = _getBuilders.GetOrAdd(type, BuildEndpointBuilder);
            return builder(variables);
        }

        public IEndpoint GetEndpoint(EndpointInfo endpointInfo)
        {
            Func<IDictionary<string, string>, IEndpoint> builder;

            if (endpointInfo.HttpMethod == "GET")
            {
                builder = _getBuilders.GetOrAdd(endpointInfo.EndpointType, BuildEndpointBuilder);
            }
            else if (endpointInfo.HttpMethod == "POST")
            {
                builder = _postBuilders.GetOrAdd(endpointInfo.EndpointType, BuildEndpointBuilder);
            }
            else
            {
                throw new HttpException(405, "Method not allowed.");
            }
            return builder(endpointInfo.Variables);
        }

        internal Func<IDictionary<string,string>, IEndpoint> BuildEndpointBuilder(Type type)
        {
            var instance = Expression.Variable(type);
            var construct = Expression.Assign(instance, Expression.New(type));
            var variables = Expression.Parameter(typeof (IDictionary<string, string>));

            var block = MakePropertySetterBlock(type, variables, instance, construct);

            return Expression.Lambda<Func<IDictionary<string, string>, IEndpoint>>(block, variables).Compile();
        }

        private static BlockExpression MakePropertySetterBlock(Type type, ParameterExpression variables,
                                                               ParameterExpression instance, BinaryExpression construct)
        {
            var lines = new List<Expression> {construct};

            var setters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Where(PropertySetterBuilder.PropertyIsPrimitive)
                .Select(p => new PropertySetterBuilder(variables, instance, p))
                .Select(ps => ps.CreatePropertySetter());

            lines.AddRange(setters);
            lines.Add(instance);

            var block = Expression.Block(type, new[] {instance}, lines);
            return block;
        }
    }
}