using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Simple.Web.Behaviors;
using Simple.Web.Http;

namespace Simple.Web.CodeGeneration
{
    class PropertyCookieSetter
    {
        private static readonly MethodInfo SetCookieValueMethod =
            typeof (PropertyCookieSetter).GetMethod("SetCookieValue", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo ToStringMethod = typeof (object).GetMethod("ToString");

        internal static IEnumerable<Expression> GetPropertyCookieSetters(Type type, ParameterExpression handler, ParameterExpression context)
        {
            foreach (var cookieProperty in type.GetProperties().Where(p => Attribute.GetCustomAttribute((MemberInfo) p, typeof(CookieAttribute)) != null))
            {
                var attribute = (CookieAttribute)Attribute.GetCustomAttribute(cookieProperty, typeof(CookieAttribute));
                var name = Expression.Constant(attribute.Name ?? cookieProperty.Name, typeof(string));
                var str = Expression.Variable(typeof (string));

                var trace = Expression.Call(typeof (Trace).GetMethod("WriteLine", new[] { typeof(object)}), Expression.Convert(handler, typeof(object)));
                var call = Expression.Call(SetCookieValueMethod, context, name,
                                           Expression.Convert(Expression.Property(handler, cookieProperty), typeof (object)));

                yield return Expression.Block(new ParameterExpression[] {str},
                                              new Expression[] { trace, call });
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