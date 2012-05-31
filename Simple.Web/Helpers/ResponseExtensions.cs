using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Helpers
{
    using Http;

    /// <summary>
    /// Extension methods for <see cref="IResponse"/>.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Sets the status code and description from a <see cref="Status"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="status">The status.</param>
        public static void SetStatus(this IResponse response, Status status)
        {
            response.StatusCode = status.Code;
            response.StatusDescription = status.Description;
        }

        /// <summary>
        /// Writes text to the response body.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="text">The text.</param>
        public static void Write(this IResponse response, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }
    }
}
