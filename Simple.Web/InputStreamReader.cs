using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using System.IO;

    class InputStreamReader
    {
        private static readonly Dictionary<string, IInputDeserializer> Deserializers = new Dictionary
            <string, IInputDeserializer>
                                                                                           {
                                                                                               {
                                                                                                   "application/x-www-form-urlencoded",
                                                                                                   new FormDeserializer()
                                                                                                   }
                                                                                           };

        public static IInputDeserializer GetDeserializer(string contentType)
        {
            return Deserializers[contentType];
        }
    }

    public interface IInputDeserializer
    {
        object Deserialize(Stream stream, Type type);
    }

    class FormDeserializer : IInputDeserializer
    {
        public object Deserialize(Stream stream, Type type)
        {
            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            var pairs = text.Split('\n').Select(s => Tuple.Create(s.Split('=')[0], s.Split('=')[1]));
            var obj = Activator.CreateInstance(type);
            foreach (var pair in pairs)
            {
                var property = type.GetProperty(pair.Item1);
                if (property != null)
                {
                    property.SetValue(obj, Convert.ChangeType(pair.Item2, property.PropertyType), null);
                }
            }
            return obj;
        }
    }
}
