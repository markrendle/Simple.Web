using Simple.Web.Behaviors;

namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    sealed class PropertySetterBuilder
    {
        private static readonly MethodInfo DictionaryContainsKeyMethod = typeof(IDictionary<string, string[]>).GetMethod("ContainsKey", new[] { typeof(string) });
        private static readonly PropertyInfo DictionaryIndexerProperty = typeof(IDictionary<string, string[]>).GetProperty("Item");

        private readonly ParameterExpression _param;
        private readonly Expression _obj;
        private readonly PropertyInfo _property;
        private MemberExpression _nameProperty;
        private Expression _itemProperty;
        private MethodCallExpression _containsKey;

        public PropertySetterBuilder(ParameterExpression param, Expression obj, PropertyInfo property)
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
            _itemProperty = Expression.ArrayIndex(Expression.Property(_param, DictionaryIndexerProperty, name), Expression.Constant(0));
        }

        private CatchBlock CreateCatchBlock()
        {
            return Expression.Catch(typeof(Exception), Expression.Assign(_nameProperty,
                                                                         Expression.Default(_property.PropertyType)));
        }

        private TryExpression CreateTrySimpleAssign()
        {
            var changeTypeMethod = typeof(PropertySetterBuilder).GetMethod("SafeConvert",
                                                                           BindingFlags.Static | BindingFlags.NonPublic);

            MethodCallExpression callConvert;
            if (_property.PropertyType.IsEnum)
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty,
                                              Expression.Constant(_property.PropertyType.GetEnumUnderlyingType()));
            }
            else if (_property.PropertyType.IsNullable())
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty,
                                              Expression.Constant(_property.PropertyType.GetGenericArguments().Single()));
            }
            else
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty,
                                              Expression.Constant(_property.PropertyType));
            }

            var assign = Expression.Assign(_nameProperty, Expression.Convert(callConvert, _property.PropertyType));
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

        public static BlockExpression MakePropertySetterBlock(Type type, ParameterExpression variables,
                                                               ParameterExpression instance, BinaryExpression construct)
        {
            var lines = new List<Expression> { construct };

            var setters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && Attribute.GetCustomAttribute(p, typeof(CookieAttribute)) == null)
                .Where(PropertyIsPrimitive)
                .Select(p => new PropertySetterBuilder(variables, instance, p))
                .Select(ps => ps.CreatePropertySetter());

            lines.AddRange(setters);
            lines.Add(instance);

            var block = Expression.Block(type, new[] { instance }, lines);
            return block;
        }
        
        public static BlockExpression MakePropertySetterBlock(Type type, MethodCallExpression getVariables,
                                                               ParameterExpression instance, BinaryExpression construct)
        {
            var variables = Expression.Variable(typeof (IDictionary<string, string[]>));
            var lines = new List<Expression> { construct };
            lines.Add(Expression.Assign(variables, getVariables));

            var setters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && Attribute.GetCustomAttribute(p, typeof(CookieAttribute)) == null)
                .Where(PropertyIsPrimitive)
                .Select(p => new PropertySetterBuilder(variables, instance, p))
                .Select(ps => ps.CreatePropertySetter());

            lines.AddRange(setters);
            lines.Add(instance);

            var block = Expression.Block(type, new[] { variables, instance }.AsEnumerable(), lines.AsEnumerable());
            return block;
        }
    }
}