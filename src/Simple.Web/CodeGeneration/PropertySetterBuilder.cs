namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Simple.Web.Behaviors;

    internal sealed class PropertySetterBuilder
    {
        private static readonly MethodInfo DictionaryContainsKeyMethod = typeof(IDictionary<string, string>).GetMethod("ContainsKey",
                                                                                                                       new[]
                                                                                                                           {
                                                                                                                               typeof(string)
                                                                                                                           });

        private static readonly PropertyInfo DictionaryIndexerProperty = typeof(IDictionary<string, string>).GetProperty("Item");

        private readonly Expression _obj;
        private readonly ParameterExpression _param;
        private readonly PropertyInfo _property;
        private MethodCallExpression _containsKey;
        private Expression _itemProperty;
        private MemberExpression _nameProperty;

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

        private CatchBlock CreateCatchBlock()
        {
            return Expression.Catch(typeof(Exception), Expression.Assign(_nameProperty, Expression.Default(_property.PropertyType)));
        }

        private void CreatePropertyExpressions()
        {
            var name = Expression.Constant(_property.Name, typeof(string));
            _containsKey = Expression.Call(_param, DictionaryContainsKeyMethod, name);
            _nameProperty = Expression.Property(_obj, _property);
            _itemProperty = Expression.Property(_param, DictionaryIndexerProperty, name);
        }

        private TryExpression CreateTrySimpleAssign()
        {
            var changeTypeMethod = typeof(PropertySetterBuilder).GetMethod("SafeConvert", BindingFlags.Static | BindingFlags.NonPublic);

            MethodCallExpression callConvert;
            if (_property.PropertyType.IsEnum)
            {
                callConvert = Expression.Call(changeTypeMethod,
                                              _itemProperty,
                                              Expression.Constant(_property.PropertyType.GetEnumUnderlyingType()));
            }
            else if (_property.PropertyType.IsNullable())
            {
                callConvert = Expression.Call(changeTypeMethod,
                                              _itemProperty,
                                              Expression.Constant(_property.PropertyType.GetGenericArguments().Single()));
            }
            else if (IsEnumerable(_property.PropertyType))
            {
                changeTypeMethod = typeof(PropertySetterBuilder).GetMethod("SafeConvertEnumerable",
                                                                           BindingFlags.Static | BindingFlags.NonPublic);
                callConvert = Expression.Call(changeTypeMethod, _itemProperty, Expression.Constant(_property.PropertyType));
            }
            else
            {
                callConvert = Expression.Call(changeTypeMethod, _itemProperty, Expression.Constant(_property.PropertyType));
            }

            var assign = Expression.Assign(_nameProperty, Expression.Convert(callConvert, _property.PropertyType));
            if (_property.PropertyType.IsEnum)
            {
                return Expression.TryCatch( // try {
                    Expression.IfThenElse(Expression.TypeIs(_itemProperty, typeof(string)),
                                          Expression.Assign(_nameProperty,
                                                            Expression.Convert(
                                                                Expression.Call(
                                                                    typeof(Enum).GetMethod("Parse",
                                                                                           new[]
                                                                                               {
                                                                                                   typeof(Type),
                                                                                                   typeof(string),
                                                                                                   typeof(bool)
                                                                                               }),
                                                                    Expression.Constant(_property.PropertyType),
                                                                    Expression.Call(_itemProperty, typeof(object).GetMethod("ToString")),
                                                                    Expression.Constant(true)),
                                                                _property.PropertyType)),
                                          assign),
                    Expression.Catch(typeof(Exception), Expression.Empty()));
            }
            if (IsAGuid(_property.PropertyType))
            {
                return Expression.TryCatch( // try {
                    Expression.IfThenElse(Expression.TypeIs(_itemProperty, typeof(string)),
                                          Expression.Assign(_nameProperty,
                                                            Expression.Convert(
                                                                Expression.Call(typeof(Guid).GetMethod("Parse", new[] { typeof(string) }),
                                                                                Expression.Call(_itemProperty,
                                                                                                typeof(object).GetMethod("ToString"))),
                                                                _property.PropertyType)),
                                          assign),
                    Expression.Catch(typeof(Exception), Expression.Empty()));
            }
            return Expression.TryCatch( // try {
                assign,
                CreateCatchBlock());
        }

        private bool PropertyIsPrimitive()
        {
            return PropertyIsPrimitive(_property);
        }

        public static BlockExpression MakePropertySetterBlock(Type type,
                                                              ParameterExpression variables,
                                                              ParameterExpression instance,
                                                              BinaryExpression construct)
        {
            var lines = new List<Expression> { construct };

            var setters =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite && Attribute.GetCustomAttribute(p, typeof(CookieAttribute)) == null)
                    .Where(PropertyIsPrimitive)
                    .Select(p => new PropertySetterBuilder(variables, instance, p))
                    .Select(ps => ps.CreatePropertySetter());

            lines.AddRange(setters);
            lines.Add(instance);

            var block = Expression.Block(type, new[] { instance }, lines);
            return block;
        }

        public static BlockExpression MakePropertySetterBlock(Type type,
                                                              MethodCallExpression getVariables,
                                                              ParameterExpression instance,
                                                              BinaryExpression construct)
        {
            var variables = Expression.Variable(typeof(IDictionary<string, string[]>));
            var lines = new List<Expression> { construct };
            lines.Add(Expression.Assign(variables, getVariables));

            var setters =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite && Attribute.GetCustomAttribute(p, typeof(CookieAttribute)) == null)
                    .Where(PropertyIsPrimitive)
                    .Select(p => new PropertySetterBuilder(variables, instance, p))
                    .Select(ps => ps.CreatePropertySetter());

            lines.AddRange(setters);
            lines.Add(instance);

            var block = Expression.Block(type, new[] { variables, instance }.AsEnumerable(), lines.AsEnumerable());
            return block;
        }

        public static bool PropertyIsPrimitive(PropertyInfo property)
        {
            return TypeIsPrimitive(property.PropertyType);
        }

        private static object Cast(object value, Type destinationType)
        {
            if (IsAGuid(destinationType))
            {
                var tmp = (string)value;
                return Guid.Parse(tmp);
            }

            if (destinationType.IsEnum)
            {
                var tmp = (string)value;
                return Enum.Parse(destinationType, tmp, true);
            }
            return Convert.ChangeType(value, destinationType);
        }

        private static bool IsAGuid(Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        private static bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && (typeof(string) != type);
        }

        private static bool IsPrimitiveEnumerable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) &&
                   TypeIsPrimitive(type.GetGenericArguments()[0]);
        }

        private static object SafeConvert(object source, Type targetType)
        {
            return ReferenceEquals(source, null) ? null : Convert.ChangeType(source, targetType);
        }

        private static object SafeConvertEnumerable(object source, Type targetType)
        {
            if (source == null)
            {
                return null;
            }

            var tmpSource = (string)source;

            var destinationType = targetType.GetGenericArguments()[0];
            var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(destinationType));
            if (!tmpSource.Contains("\t"))
            {
                // Single value IEnumerable element
                collection.Add(Cast(source, destinationType));
            }
            else
            {
                // Multi value IEnumerable element
                var parts = tmpSource.Split('\t');
                foreach (var part in parts)
                {
                    collection.Add(Cast(part, destinationType));
                }
            }

            return collection;
        }

        private static bool TypeIsPrimitive(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
                   type == typeof(Guid) || type == typeof(byte[]) || type.IsEnum ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) || IsPrimitiveEnumerable(type);
        }
    }
}