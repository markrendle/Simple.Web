using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.JsonNet
{
    using System.IO;
    using System.Linq;
    using Helpers;
    using Links;
    using MediaTypeHandling;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class JsonMediaTypeHandler : IMediaTypeHandler
    {
        private static readonly Lazy<HashSet<Type>> KnownTypes = new Lazy<HashSet<Type>>(GetKnownTypes);

        private static HashSet<Type> GetKnownTypes()
        {
            var q = ExportedTypeHelper.FromCurrentAppDomain(LinkAttributeBase.Exists)
                              .SelectMany(LinkAttributeBase.Get)
                              .Select(l => l.ModelType);
            return new HashSet<Type>(q);
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
                                                                                {
                                                                                    DateFormatHandling =
                                                                                        DateFormatHandling.IsoDateFormat,
                                                                                    ReferenceLoopHandling =
                                                                                        ReferenceLoopHandling.Ignore,
                                                                                    ContractResolver =
                                                                                        new CamelCasePropertyNamesContractResolver
                                                                                        ()
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
                var linkConverters = LinkConverter.CreateForGraph(content.Model.GetType(), KnownTypes.Value,
                                                                 LinkHelper.GetLinksForModel, Settings.ContractResolver);
                var settings = new JsonSerializerSettings
                                   {
                                       Converters = linkConverters,
                                       ContractResolver = Settings.ContractResolver,
                                       DateFormatHandling = Settings.DateFormatHandling,
                                       ReferenceLoopHandling = Settings.ReferenceLoopHandling,
                                   };
                var json = JsonConvert.SerializeObject(content.Model, settings);
                var buffer = Encoding.Default.GetBytes(json);
                return outputStream.WriteAsync(buffer, 0, buffer.Length);
            }

            return TaskHelper.Completed();
        }
    }
}
