namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for the <see cref="IResponse"/> interface.
    /// </summary>
    public static class ResponseExtensions
    {
        private const int MinusOneYear = -31557600;
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
        /// Sets a response header. Any current values for the specified header field are replaced.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="header">The header key.</param>
        /// <param name="value">The header value.</param>
        public static void SetHeader(this IResponse response, string header, string value)
        {
            EnsureHeaders(response);
            response.Headers[header] = new[] {value};
        }

        /// <summary>
        /// Adds a response header. Current values for the specified header field are retained.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="header">The header key.</param>
        /// <param name="value">The header value.</param>
        public static void AddHeader(this IResponse response, string header, string value)
        {
            EnsureHeaders(response);
            string[] currentValues;
            if (response.Headers.TryGetValue(header, out currentValues))
            {
                Array.Resize(ref currentValues, currentValues.Length + 1);
                currentValues[currentValues.Length - 1] = value;
            }
            else
            {
                currentValues = new[] {value};
            }
            response.Headers[header] = currentValues;
        }

        /// <summary>
        /// Sets a cookie.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="name">The name of the cookie to set.</param>
        /// <param name="value">The value of the cookie to set.</param>
        /// <param name="timeOut">The time (in seconds) after which the cookie expires.</param>
        /// <param name="httpOnly">A flag indicating whether the cookie is readable only via HTTP (i.e., not by client-side script). Default is <c>true</c>.</param>
        /// <param name="secure">A flag indicating whether the cookie should only be sent over HTTPS. Default is <c>false</c>.</param>
        /// <param name="path">The path below which the cookie applies.</param>
        public static void SetCookie(this IResponse response, string name, string value, int timeOut = 0, bool httpOnly = true, bool secure = false, string path = "/")
        {
            response.AddHeader(HeaderKeys.SetCookie, CookieWriter.Write(name, value, timeOut, httpOnly, secure, path));
        }

        /// <summary>
        /// Sets the ETag header.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="etag">The ETag value.</param>
        public static void SetETag(this IResponse response, string etag)
        {
            response.SetHeader(HeaderKeys.ETag, etag);
        }

        /// <summary>
        /// Removes a cookie (by setting its Expiry one year in the past).
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="name">The name of the cookie to remove.</param>
        public static void RemoveCookie(this IResponse response, string name)
        {
            EnsureHeaders(response);
            response.AddHeader(HeaderKeys.SetCookie, CookieWriter.WriteDelete(name));
        }

        /// <summary>
        /// Sets the Last-Modified header.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="dateTime">The time the resource was last modified.</param>
        public static void SetLastModified(this IResponse response, DateTime dateTime)
        {
            EnsureHeaders(response);
            response.SetHeader(HeaderKeys.LastModified, dateTime.ToUniversalTime().ToString("R"));
        }

        /// <summary>
        /// Disables response caching by setting the Cache-Control header to &quot;no-cache&amp; no-store&quot;.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        public static void DisableCache(this IResponse response)
        {
            EnsureHeaders(response);
            response.SetHeader(HeaderKeys.CacheControl, "no-cache; no-store");
        }

        /// <summary>
        /// Sets the Cache-Control header and optionally the Expires and Vary headers.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="cacheOptions">A <see cref="CacheOptions"/> object to specify the cache settings.</param>
        public static void SetCacheOptions(this IResponse response, CacheOptions cacheOptions)
        {
            if (cacheOptions == null) return;
            if (cacheOptions.Disable)
            {
                response.DisableCache();
                return;
            }
            response.SetHeader(HeaderKeys.CacheControl, cacheOptions.ToHeaderString());
            if (cacheOptions.AbsoluteExpiry.HasValue)
            {
                response.SetHeader(HeaderKeys.Expires, cacheOptions.AbsoluteExpiry.Value.ToString("R"));
            }

            if (cacheOptions.VaryByHeaders != null && cacheOptions.VaryByHeaders.Count > 0)
            {
                response.SetHeader(HeaderKeys.Vary, string.Join(", ", cacheOptions.VaryByHeaders));
            }
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