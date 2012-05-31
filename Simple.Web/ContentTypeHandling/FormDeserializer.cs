namespace Simple.Web.ContentTypeHandling
{
    using System;
    using System.IO;
    using System.Linq;

    [ContentTypes("application/x-www-form-urlencoded")]
    sealed class FormDeserializer : IContentTypeHandler
    {
        private static readonly char[] SplitTokens = new[] {'\n', '&'};

        /// <summary>
        /// Reads content from the specified input stream, which is assumed to be in x-www-form-urlencoded format.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <returns>
        /// A model constructed from the content in the input stream.
        /// </returns>
        public object Read(Stream inputStream, Type inputType)
        {
            string text;
            using (var streamReader = new StreamReader(inputStream))
            {
                text = streamReader.ReadToEnd();
            }
            var pairs = text.Split(SplitTokens, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Tuple.Create(s.Split('=')[0], Uri.UnescapeDataString(s.Split('=')[1])));
            var obj = Activator.CreateInstance(inputType);
            foreach (var pair in pairs)
            {
                var property = inputType.GetProperty(pair.Item1) ?? inputType.GetProperties().FirstOrDefault(p => p.Name.Equals(pair.Item1, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    property.SetValue(obj, Convert.ChangeType(pair.Item2, property.PropertyType), null);
                }
            }
            return obj;
        }

        /// <summary>
        /// Writes the specified content as x-www-form-urlencoded.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="outputStream">The output stream.</param>
        public void Write(IContent content, Stream outputStream)
        {
            throw new NotImplementedException();
        }
    }
}