namespace Simple.Web
{
    using System;
    using System.IO;
    using System.Linq;

    class FormDeserializer : IInputDeserializer
    {
        public object Deserialize(Stream stream, Type type)
        {
            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            var pairs = text.Split('\n').Select(s => Tuple.Create<string, string>(s.Split('=')[0], s.Split('=')[1]));
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