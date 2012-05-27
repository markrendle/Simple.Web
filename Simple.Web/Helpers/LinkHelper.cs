namespace Simple.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class LinkHelper
    {
        public static IEnumerable<Link> GetLinksForModel(object model)
        {
            if (model == null) throw new ArgumentNullException("model");

            return from type in ExportedTypeHelper.FromCurrentAppDomain(LinksFromAttribute.Exists)
                   select LinksFromAttribute.Get(type, model.GetType())
                   into forModel where forModel.Count != 0 from attribute in forModel
                   let uri = BuildUri(model, attribute) select new Link(uri, attribute.Rel, attribute.Type);
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