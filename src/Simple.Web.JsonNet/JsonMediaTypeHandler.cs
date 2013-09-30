namespace Simple.Web.JsonNet
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    using Simple.Web.Links;
    using Simple.Web.MediaTypeHandling;

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class JsonMediaTypeHandler : JsonNetMediaTypeHandlerBase
    {
        static JsonMediaTypeHandler()
        {
            SetDefaultSettings(new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });
        }

        protected override void AddWireFormattedLinks(JToken wireFormattedItem, IEnumerable<Link> itemLinks)
        {
            IList<Link> links = itemLinks as IList<Link> ?? itemLinks.ToList();
            if (links.Count == 0)
            {
                return;
            }
            var jLinks = new JArray();
            foreach (Link link in links)
            {
                EnsureLinkTypeIsJson(link);
                jLinks.Add(JObject.FromObject(link, Serializer));
            }
            wireFormattedItem["links"] = jLinks;
        }

        protected override JToken WrapCollection(IList<JToken> collection, IEnumerable<Link> collectionLinks)
        {
            var array = new JArray();
            foreach (JToken jitem in collection)
            {
                array.Add(jitem);
            }
            return array;
        }
    }
}