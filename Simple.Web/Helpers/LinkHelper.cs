namespace Simple.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Helper methods for working with RESTful links.
    /// </summary>
    public static class LinkHelper
    {
        /// <summary>
        /// Gets the links for a model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A readonly <see cref="ICollection&lt;Link&gt;"/> containing all available links for the model.</returns>
        public static ICollection<Link> GetLinksForModel(object model)
        {
            if (model == null) throw new ArgumentNullException("model");

            // There are few places where LINQ syntax is preferable. This is one of those places.
            var q = from type in ExportedTypeHelper.FromCurrentAppDomain(LinksFromAttribute.Exists)
                    let forModel = LinksFromAttribute.Get(type, model.GetType())
                    where forModel.Count != 0
                    from attribute in forModel
                    let uri = BuildUri(model, attribute)
                    select new Link(type, uri, attribute.Rel, attribute.Type);

            return new ReadOnlyCollection<Link>(q.ToArray());
        }

        private static string BuildUri(object model, LinksFromAttribute attribute)
        {
            var uri = attribute.UriTemplate;
            var variables = new HashSet<string>(UriTemplateHelper.ExtractVariableNames(attribute.UriTemplate),
                                                StringComparer.OrdinalIgnoreCase);
            if (variables.Count > 0)
            {
                foreach (var variable in variables)
                {
                    var prop = model.GetType().GetProperty(variable);
                    if (prop == null)
                    {
                        continue;
                    }
                    var value = prop.GetValue(model, null) ?? "NULL";
                    uri = uri.Replace("{" + variable + "}", value.ToString());
                }
            }
            return uri;
        }
    }
}