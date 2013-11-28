using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.JsonNet
{
    [MediaTypes(MediaType.Json, "application/*+json")]
    public class JsonMediaTypeHandler : JsonNetMediaTypeHandlerBase
    {
        private static IJsonLinksFormatter _defaultJsonLinksFormatter = new DefaultJsonLinksFormatter();

        public static IJsonLinksFormatter DefaultJsonLinksFormatter
        {
            get { return _defaultJsonLinksFormatter; }
            set { _defaultJsonLinksFormatter = value; }
        }

        private IJsonLinksFormatter _jsonLinksFormatter = _defaultJsonLinksFormatter;

        public IJsonLinksFormatter JsonLinksFormatter
        {
            get { return _jsonLinksFormatter; }
            set { _jsonLinksFormatter = value; }
        }

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
            var links = itemLinks as IList<Link> ?? itemLinks.ToList();
            if (links.Count == 0)
            {
                return;
            }
            _jsonLinksFormatter.FormatLinks((JContainer)wireFormattedItem, links.Where(EnsureLinkTypeIsJson), Serializer);
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