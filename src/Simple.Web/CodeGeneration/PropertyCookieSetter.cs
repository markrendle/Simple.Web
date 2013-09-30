namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    internal class PropertyCookieSetter
    {
        private static readonly MethodInfo SetCookieValueMethod = typeof(PropertyCookieSetter).GetMethod("SetCookieValue",
                                                                                                         BindingFlags.NonPublic |
                                                                                                         BindingFlags.Static);

        private static readonly MethodInfo SetCookieValuesMethod = typeof(PropertyCookieSetter).GetMethod("SetCookieValues",
                                                                                                          BindingFlags.NonPublic |
                                                                                                          BindingFlags.Static);

        private static readonly MethodInfo ToStringMethod = typeof(object).GetMethod("ToString");

        internal static IEnumerable<Expression> GetPropertyCookieSetters(Type type, ParameterExpression handler, ParameterExpression context)
        {
            foreach (var cookieProperty in type.GetProperties().Where(p => Attribute.GetCustomAttribute(p, typeof(CookieAttribute)) != null)
                )
            {
                var attribute = (CookieAttribute)Attribute.GetCustomAttribute(cookieProperty, typeof(CookieAttribute));
                var name = Expression.Constant(attribute.Name ?? cookieProperty.Name, typeof(string));

                if (cookieProperty.PropertyType.IsPrimitive || cookieProperty.PropertyType == typeof(string) ||
                    cookieProperty.PropertyType == typeof(Guid) || cookieProperty.PropertyType.IsNullable())
                {
                    var call = Expression.Call(SetCookieValueMethod,
                                               context,
                                               name,
                                               Expression.Convert(Expression.Property(handler, cookieProperty), typeof(object)));
                    yield return call;
                }
            }
        }

        private static void SetCookieValue(IContext context, string name, object value)
        {
            if (value == null)
            {
                context.Response.RemoveCookie(name);
            }
            else
            {
                context.Response.SetCookie(name, value.ToString());
            }
        }
    }
}