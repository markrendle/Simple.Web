namespace Simple.Web.JsonNet
{
    using System.Collections.Generic;
    using Links;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Formats links in JSON using a light-weight object format.
    /// </summary>
    /// <example>
    /// A customer object might be formatted like this:
    /// 
    /// {"name":"ACME","_links":{"self":"/customers/acme","orders":"customers/acme/orders"}}
    /// </example>
    public class SimpleJsonLinksFormatter : IJsonLinksFormatter
    {
        private string _linksPropertyName = "_links";

        public string LinksPropertyName
        {
            get { return _linksPropertyName; }
            set { _linksPropertyName = value; }
        }

        public void FormatLinks(JContainer container, IEnumerable<Link> links, JsonSerializer serializer)
        {
            var jLinks = new JObject();
            foreach (var link in links)
            {
                jLinks[link.Rel] = JValue.CreateString(link.Href);
            }
            container[_linksPropertyName] = jLinks;
        }
    }
}