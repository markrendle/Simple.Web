namespace Simple.Web.MediaTypeHandling
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using Simple.Web.Helpers;

    [MediaTypes("application/x-www-form-urlencoded")]
    internal sealed class FormDeserializer : IMediaTypeHandler
    {
        private static readonly char[] SplitTokens = { '\n', '&' };

        /// <summary>
        /// Reads content from the specified input stream, which is assumed to be in x-www-form-urlencoded format.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>
        /// A model constructed from the content in the input stream.
        /// </returns>
        public Task<T> Read<T>(Stream inputStream)
        {
            string text;
            using (var streamReader = new StreamReader(inputStream))
            {
                text = streamReader.ReadToEnd();
            }
            var pairs = text.Split(SplitTokens, StringSplitOptions.RemoveEmptyEntries);
            var obj = Activator.CreateInstance<T>();
            // reflection is slow, get the property[] once.
            var properties = typeof(T).GetProperties();
            foreach (var pair in pairs)
            {
                var nameValue = pair.Split('=');
                var property = properties.FirstOrDefault(p => p.Name.Equals(nameValue[0], StringComparison.Ordinal)) ??
                               properties.FirstOrDefault(p => p.Name.Equals(nameValue[0], StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    property.SetValue(obj, Convert.ChangeType(HttpUtility.UrlDecode(nameValue[1]), property.PropertyType), null);
                }
            }
            return TaskHelper.Completed(obj);
        }

        /// <summary>
        /// Writes the specified content as x-www-form-urlencoded.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="outputStream">The output stream.</param>
        public Task Write<T>(IContent content, Stream outputStream)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new NotSupportedException());
            return tcs.Task;
        }
    }
}