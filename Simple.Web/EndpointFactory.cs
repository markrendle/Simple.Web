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
            var builder = _getBuilders.GetOrAdd(type, BuildGetBuilder);
            return builder(variables);
        }

        public IEndpoint PostEndpoint(Type endpointType, IDictionary<string, string> variables, HttpRequest request)
        {
            var modelType = GetModelType(endpointType);
            var model = CreateModel(modelType, request);
            var builder = _postBuilders.GetOrAdd(endpointType, BuildPostBuilder);
            var endpoint = builder(variables);
            endpointType.GetProperty("Model").SetValue(endpoint, model, null);
            return endpoint;
        }

        private object CreateModel(Type modelType, HttpRequest request)
        {
            if (request.InputStream.Length == 0) return null;
            return InputStreamReader.GetDeserializer(request.ContentType).Deserialize(request.InputStream,
                                                                                           modelType);
        }

        private static Type GetModelType(Type postEndpointType)
        {
            var baseType = postEndpointType.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(PostEndpoint<,>))
                {
                    return baseType.GetGenericArguments()[0];
                }
                baseType = baseType.BaseType;
            }
            throw new InvalidOperationException("Type does not derive from PostEndpoint<TRequest,TResponse>");
        }

        private Func<IDictionary<string, string>, IEndpoint> BuildPostBuilder(Type type)
        {
            var instance = Expression.Variable(type);
            var construct = Expression.Assign(instance, Expression.New(type));
            var variables = Expression.Parameter(typeof (IDictionary<string, string>));

            var block = MakePropertySetterBlock(type, variables, instance, construct);

            return Expression.Lambda<Func<IDictionary<string, string>, IEndpoint>>(block, variables).Compile();
        }

        internal Func<IDictionary<string,string>, IEndpoint> BuildGetBuilder(Type type)
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