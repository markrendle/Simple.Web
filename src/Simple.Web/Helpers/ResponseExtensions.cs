namespace Simple.Web.Helpers
{
    using System.Text;

    using Simple.Web.Http;

    /// <summary>
    /// Extension methods for <see cref="IResponse"/>.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Writes text to the response body.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="text">The text.</param>
        public static void Write(this IResponse response, string text)
        {
            response.WriteFunction = stream =>
                                     {
                                         var bytes = Encoding.UTF8.GetBytes(text);
                                         return stream.WriteAsync(bytes, 0, bytes.Length);
                                     };
        }
    }
}