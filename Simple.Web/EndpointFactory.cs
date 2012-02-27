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
        internal EndpointFactory()
        {
            
        }

        private static EndpointFactory _instance;
        public static EndpointFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EndpointFactory();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, GetEndpoint>> _builders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, GetEndpoint>>();
        private readonly Dictionary<string, Type> _getEndpointTypes = new Dictionary<string, Type>();

        private void Initialize()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var exportedType in assembly.GetExportedTypes().Where(type => typeof(GetEndpoint).IsAssignableFrom(type) && !type.IsAbstract))
                {
                    var instance = Activator.CreateInstance(exportedType) as GetEndpoint;
                    if (instance != null)
                    {
                        _getEndpointTypes.Add(instance.UriTemplate, exportedType);
                    }
                }
            }
        }

        public GetEndpoint GetEndpoint(string absolutePath)
        {
            if (_getEndpointTypes.ContainsKey(absolutePath))
            {
                return (GetEndpoint)Activator.CreateInstance(_getEndpointTypes[absolutePath]);
            }
            return null;
        }

        public GetEndpoint GetEndpoint(Type type, IDictionary<string,string> variables)
        {
            var builder = _builders.GetOrAdd(type, BuildBuilder);
            return builder(variables);
        }

        internal Func<IDictionary<string,string>, GetEndpoint> BuildBuilder(Type type)
        {
            var instance = Expression.Variable(type);
            var variables = Expression.Parameter(typeof (IDictionary<string, string>));
            var construct = Expression.Assign(instance, Expression.New(type));

            var lines = new List<Expression> { construct };

            var setters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Where(PropertySetterBuilder.PropertyIsPrimitive)
                .Select(p => new PropertySetterBuilder(variables, instance, p))
                .Select(ps => ps.CreatePropertySetter());

            lines.AddRange(setters);
            lines.Add(instance);

            var block = Expression.Block(typeof (GetEndpoint), new[] {instance}, lines);

            return Expression.Lambda<Func<IDictionary<string, string>, GetEndpoint>>(block, variables).Compile();
        }
    }

    class PropertySetterBuilder
    {
        private static readonly MethodInfo DictionaryContainsKeyMethod = typeof(IDictionary<string, string>).GetMethod("ContainsKey", new[] { typeof(string) });
        private static readonly PropertyInfo DictionaryIndexerProperty = typeof(IDictionary<string, string>).GetProperty("Item");

        private readonly ParameterExpression _param;
        private readonly ParameterExpression _obj;
        private readonly PropertyInfo _property;
        private MemberExpression _nameProperty;
        private IndexExpression _itemProperty;
        private MethodCallExpression _containsKey;

        public PropertySetterBuilder(ParameterExpression param, ParameterExpression obj, PropertyInfo property)
        {
            _param = param;
            _obj = obj;
            _property = property;
        }

        public ConditionalExpression CreatePropertySetter()
        {
            CreatePropertyExpressions();

            if (PropertyIsPrimitive())
            {
                return Expression.IfThen(_containsKey, CreateTrySimpleAssign());
            }

            return null;
        }

        private bool PropertyIsPrimitive()
        {
            return PropertyIsPrimitive(_property);
        }

        public static bool PropertyIsPrimitive(PropertyInfo property)
        {
            return property.PropertyType.IsPrimitive || property.PropertyType == typeof(string) ||
                   property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(byte[]) ||
                   property.PropertyType.IsEnum ||
                   (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>));
            
        }

        private void CreatePropertyExpressions()
        {
            var name = Expression.Constant(_property.Name, typeof(string));
            _containsKey = Expression.Call(_param, DictionaryContainsKeyMethod, name);
            _nameProperty = Expression.Property(_obj, _property);
            _itemProperty = Expression.Property(_param, DictionaryIndexerProperty, name);
        }

        private CatchBlock CreateCatchBlock()
        {
            return Expression.Catch(typeof(Exception), Expression.Assign(_nameProperty,
                                                                         Expression.Default(_property.PropertyType)));
        }

        private TryExpression CreateTrySimpleAssign()
        {
            Expression assign;
            var changeTypeMethod = typeof(PropertySetterBuilder).GetMethod("SafeConvert",
                                                                            BindingFlags.Static | BindingFlags.NonPublic);

            MethodCallExpression callConvert;
            if (_property.PropertyType.IsEnum)
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty,
                                              Expression.Constant(_property.PropertyType.GetEnumUnderlyingType()));
            }
            else if (_property.PropertyType.IsGenericType && _property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty,
                                              Expression.Constant(_property.PropertyType.GetGenericArguments().Single()));
            }
            else
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty,
                                              Expression.Constant(_property.PropertyType));
            }

            assign = Expression.Assign(_nameProperty, Expression.Convert(callConvert, _property.PropertyType));
            if (_property.PropertyType.IsEnum)
            {
                return Expression.TryCatch( // try {
                    Expression.IfThenElse(Expression.TypeIs(_itemProperty, typeof(string)),
                                          Expression.Assign(_nameProperty,
                                                            Expression.Convert(Expression.Call(typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) }),
                                                                                               Expression.Constant(_property.PropertyType),
                                                                                               Expression.Call(_itemProperty, typeof(object).GetMethod("ToString")), Expression.Constant(true)), _property.PropertyType)),
                                          assign), Expression.Catch(typeof(Exception), Expression.Empty()));
            }
            return Expression.TryCatch( // try {
                assign,
                CreateCatchBlock());
        }

        private static object SafeConvert(object source, Type targetType)
        {
            return ReferenceEquals(source, null) ? null : Convert.ChangeType(source, targetType);
        }
    }
}