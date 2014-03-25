namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Behaviors;
    using Http;

    class PropertyHeaderSetter
    {
        private static readonly MethodInfo SetHeaderValueMethod =
            typeof (PropertyHeaderSetter).GetMethod("SetHeaderValue", BindingFlags.NonPublic | BindingFlags.Static);

        internal static IEnumerable<Expression> GetPropertyHeaderSetters(Type type, ParameterExpression handler, ParameterExpression context)
        {
            foreach (var headerProperty in type.GetProperties().Where(p => Attribute.GetCustomAttribute(p, typeof(ResponseHeaderAttribute)) != null))
            {
                var attribute = (ResponseHeaderAttribute)Attribute.GetCustomAttribute(headerProperty, typeof(ResponseHeaderAttribute));
                var name = Expression.Constant(attribute.FieldName ?? headerProperty.Name, typeof(string));

                var call = Expression.Call(SetHeaderValueMethod, context, name,
                    Expression.Convert(Expression.Property(handler, headerProperty),
                        typeof (object)));
                yield return call;
            }
        }       

        private static void SetHeaderValue(IContext context, string name, object value)
        {
            if (value == null) return;
            context.Response.SetHeader(name, value.ToString());
        }
    }
}