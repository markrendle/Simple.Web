namespace Simple.Web.JsonNet
{
    using System.Collections.Generic;
    using Links;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class DefaultJsonLinksFormatter : IJsonLinksFormatter
    {
        private string _linksPropertyName = "links";

        public string LinksPropertyName
        {
            get { return _linksPropertyName; }
            set { _linksPropertyName = value; }
        }

        public void FormatLinks(JContainer container, IEnumerable<Link> links, JsonSerializer serializer)
        {
            var jLinks = new JArray();
            foreach (var link in links)
            {
                jLinks.Add(JObject.FromObject(link, serializer));
            }
            container[_linksPropertyName] = jLinks;
        }
    }
}