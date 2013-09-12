using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.JsonNet
{
    [MediaTypes("application/hal+json")]
    public class HalJsonMediaTypeHandler : JsonNetMediaTypeHandlerBase
    {
        static HalJsonMediaTypeHandler()
        {
            SetDefaultSettings(new JsonSerializerSettings
                {
                    //DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        protected override void AddWireFormattedLinks(JToken wireFormattedItem, IEnumerable<Link> itemLinks)
        {
            IList<Link> links = itemLinks as IList<Link> ?? itemLinks.ToList();
            if (links.Count == 0)
            {
                return;
            }
            JObject halLinks = CreateLinks(links);
            wireFormattedItem["_links"] = halLinks;
        }

        protected override JToken WrapCollection(IList<JToken> collection, IEnumerable<Link> collectionLinks)
        {
            var container = new JObject();
            var array = new JArray();
            container["collection"] = array;
            foreach (JToken jitem in collection)
            {
                array.Add(jitem);
            }
            container["_links"] = CreateLinks(collectionLinks);
            return container;
        }

        private JObject CreateLinks(IEnumerable<Link> itemLinks)
        {
            var halLinks = new JObject();
            foreach (Link link in itemLinks)
            {
                halLinks.Add(link.Rel, JObject.FromObject(new HalLink(link.Href, link.Title), Serializer));
            }
            return halLinks;
        }

        private class HalLink
        {
            public HalLink(string href, string title)
            {
                if (href == null) throw new ArgumentNullException("href");
                Href = href;
                Title = string.IsNullOrWhiteSpace(title) ? null : title;
            }

            public string Href { get; private set; }

            public string Title { get; private set; }
        }
    }
}