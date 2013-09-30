namespace Simple.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Helper class to build URIs from a Handler's type
    /// </summary>
    public static class UriFromType
    {
        /// <summary>
        /// Gets the URI for a handler without template parameters.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <returns>The URI.</returns>
        public static Uri Get<THandler>()
        {
            return Get(typeof(THandler));
        }

        /// <summary>
        /// Gets the URI for a handler without template parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The URI.</returns>
        public static Uri Get(Type type)
        {
            var uriTemplateAttributes = UriTemplateAttribute.Get(type).ToArray();
            AssertAtLeastOne(uriTemplateAttributes);
            AssertSingle(uriTemplateAttributes);

            var template = uriTemplateAttributes[0].Template;

            if (template.Contains("{"))
            {
                throw new InvalidOperationException(
                    "UriTemplateAttribute includes variables. Use the Get<THandler>(Expression<Func<T>>) override.");
            }

            return new Uri(template, UriKind.Relative);
        }

        /// <summary>
        /// Gets the URI for a handler with template parameters.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="expression">An expression specifying values for properties on the Handler.</param>
        /// <returns>A URI with the necessary values incorporated.</returns>
        public static Uri Get<THandler>(Expression<Func<THandler>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var memberInitExpression = expression.Body as MemberInitExpression;
            if (memberInitExpression == null)
            {
                throw new ArgumentException("Expression must be a member initializer.");
            }

            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (var memberAssignment in memberInitExpression.Bindings.OfType<MemberAssignment>())
            {
                object value;
                if (ExpressionHelper.TryGetValue(memberAssignment.Expression, out value))
                {
                    values.Add(memberAssignment.Member.Name, value);
                }
                else
                {
                    throw new ArgumentException(
                        "Cannot resolve values from member initializer. Make sure you are not calling a function within the expression.");
                }
            }

            var uriTemplateAttributes = UriTemplateAttribute.Get(typeof(THandler)).ToArray();
            AssertAtLeastOne(uriTemplateAttributes);

            foreach (var uriTemplateAttribute in uriTemplateAttributes)
            {
                var template = uriTemplateAttribute.Template;
                var variables = new HashSet<string>(UriTemplateHelper.ExtractVariableNames(template), StringComparer.OrdinalIgnoreCase);
                if (variables.All(values.ContainsKey))
                {
                    var uri = uriTemplateAttribute.Template;
                    foreach (var variable in variables)
                    {
                        uri = uri.Replace("{" + variable + "}", (values[variable] ?? string.Empty).ToString());
                    }
                    return new Uri(uri, UriKind.Relative);
                }
            }

            throw new InvalidOperationException("Cannot find matching Uri template.");
        }

        private static void AssertAtLeastOne(IList<UriTemplateAttribute> attributes)
        {
            if (attributes.Count == 0)
            {
                throw new InvalidOperationException("No UriTemplateAttribute found on type.");
            }
        }

        private static void AssertSingle(IList<UriTemplateAttribute> attributes)
        {
            if (attributes.Count > 1)
            {
                throw new InvalidOperationException("Multiple UriTemplateAttribute instances found on type.");
            }
        }
    }
}