namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    internal class CookiePropertySetter
    {
        private static readonly MethodInfo GetCookieValueMethod = typeof(CookiePropertySetter).GetMethod("GetCookieValue",
                                                                                                         BindingFlags.Static |
                                                                                                         BindingFlags.NonPublic);

        internal static IEnumerable<Expression> GetCookiePropertySetters(Type type, ParameterExpression handler, ParameterExpression context)
        {
            foreach (var cookieProperty in
                type.GetProperties().Where(p => p.CanWrite && Attribute.GetCustomAttribute(p, typeof(CookieAttribute)) != null))
            {
                var attribute = (CookieAttribute)Attribute.GetCustomAttribute(cookieProperty, typeof(CookieAttribute));
                var property = Expression.Property(handler, cookieProperty);
                var name = Expression.Constant(attribute.Name ?? cookieProperty.Name, typeof(string));

                var assignment = CreateAssignment(cookieProperty, property, Expression.Call(GetCookieValueMethod, context, name));
//                    ?? CreateComplexAssignment(cookieProperty, property, Expression.Call(GetCookieValuesMethod, context, name));

                if (assignment != null)
                {
                    yield return assignment;
                }
            }
        }

        private static Expression CreateAssignment(PropertyInfo cookieProperty,
                                                   MemberExpression property,
                                                   MethodCallExpression getCookieValue)
        {
            Expression assignment = null;

            if (cookieProperty.PropertyType == typeof(string))
            {
                assignment = Expression.IfThen(Expression.NotEqual(getCookieValue, Expression.Constant(null, typeof(string))),
                                               Expression.Assign(property, getCookieValue));
            }
            else
            {
                var parseMethod = GetParseMethod(cookieProperty);
                if (parseMethod != null)
                {
                    assignment = CreateParseAssignment(cookieProperty, property, getCookieValue, parseMethod);
                }
            }
            return assignment;
        }

        //private static Expression CreateComplexAssignment(PropertyInfo cookieProperty, MemberExpression property, MethodCallExpression getCookieValues)
        //{
        //    var ctor = cookieProperty.PropertyType.GetConstructor(new Type[0]);
        //    if (ctor == null) return null;
        //    var instance = Expression.Variable(cookieProperty.PropertyType);
        //    var construct = Expression.Assign(instance, Expression.New(ctor));
        //    var setterBlock = PropertySetterBuilder.MakePropertySetterBlock(cookieProperty.PropertyType, getCookieValues, instance, construct);
        //    return Expression.IfThen(Expression.NotEqual(getCookieValues, Expression.Constant(null, typeof(IDictionary<string,string>))),  Expression.Assign(property, setterBlock));
        //}

        private static Expression CreateParseAssignment(PropertyInfo cookieProperty,
                                                        MemberExpression property,
                                                        MethodCallExpression getCookieValue,
                                                        MethodInfo parseMethod)
        {
            Expression callExpression = Expression.Call(parseMethod, getCookieValue);
            if (parseMethod.ReturnType != cookieProperty.PropertyType)
            {
                callExpression = Expression.Convert(callExpression, cookieProperty.PropertyType);
            }
            return
                Expression.TryCatch(
                    Expression.IfThenElse(Expression.NotEqual(getCookieValue, Expression.Constant(null, typeof(string))),
                                          Expression.Assign(property, callExpression),
                                          Expression.Default(cookieProperty.PropertyType)),
                    Expression.Catch(typeof(Exception), Expression.Empty()));
        }

        private static string GetCookieValue(IContext context, string name)
        {
            string value;
            context.Request.TryGetCookieValue(name, out value);
            Trace.WriteLine("GetCookieValue");
            return value;
        }

        private static MethodInfo GetParseMethod(PropertyInfo cookieProperty)
        {
            var type = cookieProperty.PropertyType;

            if (type.IsNullable())
            {
                type = cookieProperty.PropertyType.GetGenericArguments()[0];
            }
            return type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null);
        }
    }
}