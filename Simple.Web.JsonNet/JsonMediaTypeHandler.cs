using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.JsonNet
{
    using System.IO;
    using Helpers;
    using Links;
    using MediaTypeHandling;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class JsonMediaTypeHandler : IMediaTypeHandler
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
                                                                      {
                                                                          DateFormatHandling =
                                                                              DateFormatHandling.IsoDateFormat,
                                                                          ContractResolver =
                                                                              new CamelCasePropertyNamesContractResolver()
                                                                      };

        public static JsonSerializerSettings Settings
        {
            get { return SerializerSettings; }
        }

        public object Read(Stream inputStream, Type inputType)
        {
            // pass the combined resolver strategy into the settings object
            using (var streamReader = new StreamReader(inputStream))
            {
                return JsonConvert.DeserializeObject(streamReader.ReadToEnd(), inputType, SerializerSettings);
            }
        }

        public Task Write(IContent content, Stream outputStream)
        {
            if (content.Model != null)
            {
                object output;

                var enumerable = content.Model as IEnumerable<object>;
                if (enumerable != null)
                {
                    output = ProcessList(enumerable.ToList());
                }
                else
                {
                    output = ProcessContent(content);
                }
                var json = JsonConvert.SerializeObject(output, SerializerSettings);
                var buffer = Encoding.Default.GetBytes(json);
                return outputStream.WriteAsync(buffer, 0, buffer.Length);
            }

            return TaskHelper.Completed();
        }

        private static object ProcessContent(IContent content)
        {
            var links = content.Links.Select(EnsureJson).ToList();
            if (links.Count == 0)
            {
                return content.Model;
            }
            var dictionary = content.Model.ToDictionary();
            dictionary.Add("links", links);
            return dictionary;
        }

        private static IEnumerable<object> ProcessList(IEnumerable<object> source)
        {
            bool skipLinkCheck = false;
            foreach (var o in source)
            {
                if (!skipLinkCheck)
                {
                    var links = LinkHelper.GetLinksForModel(o).Select(EnsureJson).ToList();
                    if (links.Count == 0)
                    {
                        skipLinkCheck = true;
                    }
                    else
                    {
                        var dictionary = o.ToDictionary();
                        dictionary.Add("links", links);
                        yield return dictionary;
                        continue;
                    }

                }

                yield return o;
            }
        }

        private static Link EnsureJson(Link source)
        {
            if (!string.IsNullOrWhiteSpace(source.Type))
            {
                if (source.Type.EndsWith("json"))
                {
                    return source;
                }
                return new Link(source.GetHandlerType(), source.Href, source.Rel, source.Type + "+json", source.Title);
            }
            return new Link(source.GetHandlerType(), source.Href, source.Rel, MediaType.Json, source.Title);
        }
    }
}
