using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Web
{
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    class EndpointFactory
    {
        private static EndpointFactory _instance;
        public static EndpointFactory Instance
        {
            get { return _instance ?? (_instance = new EndpointFactory()); }
        }

        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, GetEndpoint>> _getBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, GetEndpoint>>();
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, PostEndpoint>> _postBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, PostEndpoint>>();

        public GetEndpoint GetEndpoint(Type type, IDictionary<string,string> variables)
        {
            var builder = _getBuilders.GetOrAdd(type, BuildGetBuilder);
            return builder(variables);
        }

        public PostEndpoint PostEndpoint(Type type, IDictionary<string,string> variables)
        {
            var builder = _postBuilders.GetOrAdd(type, BuildPostBuilder);
            return builder(variables);
        }

        private Func<IDictionary<string, string>, PostEndpoint> BuildPostBuilder(Type type)
        {
            var instance = Expression.Variable(type);
            var construct = Expression.Assign(instance, Expression.New(type));
            var variables = Expression.Parameter(typeof (IDictionary<string, string>));

            var block = MakePropertySetterBlock(type, variables, instance, construct);

            return Expression.Lambda<Func<IDictionary<string, string>, PostEndpoint>>(block, variables).Compile();
        }

        internal Func<IDictionary<string,string>, GetEndpoint> BuildGetBuilder(Type type)
        {
            var instance = Expression.Variable(type);
            var construct = Expression.Assign(instance, Expression.New(type));
            var variables = Expression.Parameter(typeof (IDictionary<string, string>));

            var block = MakePropertySetterBlock(type, variables, instance, construct);

            return Expression.Lambda<Func<IDictionary<string, string>, GetEndpoint>>(block, variables).Compile();
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