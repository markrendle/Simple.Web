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
        Link CanonicalForModel(object model);
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
                _templates.Select(l => new Link(l.GetHandlerType(), BuildUri(model, l.Href), l.Rel, l.Type, l.Title)).ToList();
            return new ReadOnlyCollection<Link>(actuals);
        }
        
        public Link CanonicalForModel(object model)
        {
            return
                _templates.Where(t => t.Rel == "self").Select(
                    l => new Link(l.GetHandlerType(), BuildUri(model, l.Href), l.Rel, l.Type, l.Title)).FirstOrDefault();
        }

        private static string BuildUri(object model, string uriTemplate)
        {
            int queryStart = uriTemplate.IndexOf("?", StringComparison.Ordinal);
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
                    var sub = "{" + variable + "}";
                    var value = prop.GetValue(model, null).ToString() ?? "NULL";
                    if (queryStart >= 0)
                    {
                        if (uriTemplate.IndexOf(sub, StringComparison.OrdinalIgnoreCase) > queryStart)
                        {
                            value = Uri.EscapeDataString(value);
                        }
                    }
                    uri.Replace(sub, value);
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

            public Link CanonicalForModel(object model)
            {
                return null;
            }
        }
    }
}