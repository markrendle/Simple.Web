namespace Simple.Web.JsonNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using Links;
    using MediaTypeHandling;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    [MediaTypes("application/hal+json")]
    public class HalJsonMediaTypeHandler : IMediaTypeHandler
    {
        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
                                                                             {
                                                                                 ReferenceLoopHandling =
                                                                                     ReferenceLoopHandling.Ignore,
                                                                                 ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                                                 NullValueHandling = NullValueHandling.Ignore,
                                                                             };

        private static JsonSerializerSettings _settings;

        public object Read(Stream inputStream, Type inputType)
        {
            var serializer = JsonSerializer.Create(Settings);
            var streamReader = new StreamReader(inputStream);
            var reader = new JsonTextReader(streamReader);
            return serializer.Deserialize(reader, inputType);
        }

        public Task Write(IContent content, Stream outputStream)
        {
            if (ReferenceEquals(null, content.Model)) return TaskHelper.Completed();

            var serializer = JsonSerializer.Create(Settings);
            var output = ProcessContent(content, serializer);

            var stringWriter = new StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            serializer.Serialize(writer, output);
            var buffer = Encoding.Default.GetBytes(stringWriter.ToString());
            return outputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static JsonSerializerSettings Settings
        {
            get { return _settings ?? DefaultSettings; }
            set { _settings = value; }
        }

        private static JObject ProcessContent(IContent content, JsonSerializer serializer)
        {
            var links = content.Links.ToList();

            JObject jo;

            var list = content.Model as IEnumerable;
            if (list != null)
            {
                jo = new JObject();
                var array = new JArray();
                jo["collection"] = array;
                foreach (var o in list)
                {
                    var jitem = JObject.FromObject(o, serializer);
                    jitem.Add("_links", CreateHalLinks(LinkHelper.GetLinksForModel(o), serializer));
                    array.Add(jitem);
                }
            }
            else
            {
                jo = JObject.FromObject(content.Model, serializer);
            }

            if (links.Count > 0)
            {
                jo["_links"] = CreateHalLinks(links, serializer);
            }
            return jo;
        }

        private static JObject CreateHalLinks(IEnumerable<Link> links, JsonSerializer serializer)
        {
            var halLinks = new JObject();
            foreach (var link in links)
            {
                halLinks.Add(link.Rel, JObject.FromObject(new HalLink(link.Href, link.Title), serializer));
            }
            return halLinks;
        }

        private class HalLink
        {
            private readonly string _href;
            private readonly string _title;

            public HalLink(string href, string title)
            {
                if (href == null) throw new ArgumentNullException("href");
                _href = href;
                _title = string.IsNullOrWhiteSpace(title) ? null : title;
            }

            public string Href
            {
                get { return _href; }
            }

            public string Title
            {
                get { return _title; }
            }
        }
    }
}