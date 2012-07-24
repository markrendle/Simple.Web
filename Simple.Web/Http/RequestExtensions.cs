namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="IRequest"/> interface.
    /// </summary>
    public static class RequestExtensions
    {
        private static readonly string[] MediaTypeWildcard = new[] {"*/*"};

        /// <summary>
        /// Gets the Accept header entry.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/> instance.</param>
        /// <returns>The Accept header value(s), or a wildcard if not in the Headers collection.</returns>
        public static IList<string> GetAccept(this IRequest request)
        {
            string[] accept;
            if (request.Headers == null || !request.Headers.TryGetValue(HeaderKeys.Accept, out accept))
            {
                accept = MediaTypeWildcard;
            }
            return accept;
        }

        /// <summary>
        /// Gets the Content-Type header entry.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/> instance.</param>
        /// <returns>The Content-Type header value, or a wildcard if not in the Headers collection.</returns>
        public static string GetContentType(this IRequest request)
        {
            string[] contentType;
            if (request.Headers == null || !request.Headers.TryGetValue(HeaderKeys.ContentType, out contentType))
            {
                contentType = MediaTypeWildcard;
            }
            return contentType.FirstOrDefault();
        }
    }

    /// <summary>
    /// Extension methods for the <see cref="IResponse"/> interface.
    /// </summary>
    public static class ResponseExtensions
    {
        private static readonly string[] MediaTypeWildcard = new[] {"*/*"};

        /// <summary>
        /// Sets the response Content-Type header.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="contentType">The content type. This should be a valid media type.</param>
        public static void SetContentType(this IResponse response, string contentType)
        {
            response.SetHeader(HeaderKeys.ContentType, contentType);
        }

        /// <summary>
        /// Sets a response header.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="header">The header key.</param>
        /// <param name="value">The header value.</param>
        public static void SetHeader(this IResponse response, string header, string value)
        {
            EnsureHeaders(response);
            response.Headers[header] = new[] {value};
        }

        private static void EnsureHeaders(IResponse response)
        {
            if (response.Headers == null)
            {
                response.Headers = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}