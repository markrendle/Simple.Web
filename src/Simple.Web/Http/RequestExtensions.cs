namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

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
            if (request.Headers == null || (!request.Headers.TryGetValue(HeaderKeys.Accept, out accept)) || accept[0].StartsWith("*/*"))
            {
                accept = MediaTypeWildcard;
            }
            else
            {
                accept = accept.SelectMany(line => line.Split(',').Select(s => s.Trim())).ToArray();
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
                return null;
            }
            return contentType.FirstOrDefault();
        }

        /// <summary>
        /// Tries to get the value of a Cookie.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/> instance.</param>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie if found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the cookie is found in the request; otherwise, <c>false</c>.</returns>
        public static bool TryGetCookieValue(this IRequest request, string name, out string value)
        {
            string[] cookies;
            if (request.Headers != null && request.Headers.TryGetValue(HeaderKeys.Cookie, out cookies))
            {
                cookies = cookies.SelectMany(c => c.Split(';').Select(s => s.Trim())).ToArray();
                var cookie =
                    cookies.FirstOrDefault(c => c.StartsWith(name + "=", StringComparison.InvariantCultureIgnoreCase));
                if (cookie != null)
                {
                    value = GetCookieValue(cookie);
                    return true;
                }
            }
            value = null;
            return false;
        }

        internal static string GetCookieValue(string cookie)
        {
            int from = cookie.IndexOf('=');
            return HttpUtility.UrlDecode(cookie.Substring(from + 1));
        }
    }
}