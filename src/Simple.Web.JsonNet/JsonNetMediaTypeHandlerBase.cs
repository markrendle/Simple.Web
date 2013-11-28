using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.Web.Helpers;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.JsonNet
{
    public abstract class JsonNetMediaTypeHandlerBase : MediaTypeHandlerBase<JToken>
    {
        private static JsonSerializerSettings _defaultSettings;
        private static JsonSerializerSettings _settings;
        private JsonSerializer _serializer;

        public static JsonSerializerSettings Settings
        {
            get { return _settings ?? _defaultSettings; }
            set { _settings = value; }
        }

        protected JsonSerializer Serializer
        {
            get { return _serializer ?? (_serializer = JsonSerializer.Create(Settings)); }
        }

        protected bool EnsureLinkTypeIsJson(Link link)
        {
            if (String.IsNullOrWhiteSpace(link.Type))
            {
                link.Type = MediaType.Json;
            }
            if (!link.Type.EndsWith("json"))
            {
                link.Type += "+json";
            }
            return true;
        }

        protected override Task<T> FromWireFormat<T>(JToken wireFormat)
        {
            JsonReader reader = new JTokenReader(wireFormat);
            var result = Serializer.Deserialize<T>(reader);
            return TaskHelper.Completed(result);
        }

        protected override Task<JToken> ReadInput(Stream inputStream)
        {
            string input;
            using (var reader = new StreamReader(inputStream))
            {
                input = reader.ReadToEnd();
            }
            JObject jObject = JObject.Parse(input);
            return TaskHelper.Completed<JToken>(jObject);
        }

        protected override JToken ToWireFormat(object item)
        {
            return JObject.FromObject(item, Serializer);
        }

        protected override JToken ToWireFormat<T>(T item)
        {
            return JObject.FromObject(item, Serializer);
        }

        protected override Task WriteOutput(JToken output, Stream outputStream)
        {
            string outputText = output.ToString(Settings.Formatting,
                                                Settings.Converters.ToArray());
            byte[] buffer = Encoding.UTF8.GetBytes(outputText);
            return outputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        protected static void SetDefaultSettings(JsonSerializerSettings defaultSettings)
        {
            _defaultSettings = defaultSettings;
        }
    }
}