namespace Simple.Web.Links
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Helpers;

    internal interface ILinkBuilder
    {
        ICollection<Link> LinksForModel(object model);
    }

    internal class LinkBuilder : ILinkBuilder
    {
        public static readonly ILinkBuilder Empty = new EmptyLinkBuilder();
        private readonly IList<Link> _templates;

        public LinkBuilder(IEnumerable<Link> templates)
        {
            _templates = templates.ToArray();
        }

        public ICollection<Link> LinksForModel(object model)
        {
            var actuals =
                _templates.Select(l => new Link(l.HandlerType, BuildUri(model, l.Href), l.Rel, l.Type)).ToList();
            return new ReadOnlyCollection<Link>(actuals);
        }

        private static string BuildUri(object model, string uriTemplate)
        {
            var uri = new StringBuilder(uriTemplate);
            var variables = new HashSet<string>(UriTemplateHelper.ExtractVariableNames(uriTemplate),
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
                    uri.Replace("{" + variable + "}", value.ToString());
                }
            }
            return uri.ToString();
        }

        private class EmptyLinkBuilder : ILinkBuilder
        {
            private static readonly Link[] EmptyArray = new Link[0];
            public ICollection<Link> LinksForModel(object model)
            {
                return EmptyArray;
            }
        }
    }
}