using System.Collections;

namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstraction for an HTTP response, to be implemented by hosting.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        string Status { get; set; }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        //Stream OutputStream { get; }

        Func<Stream, CancellationToken, Task> WriteFunction { get; set; }

            /// <summary>
        /// The response headers.
        /// </summary>
        IDictionary<string, string[]> Headers { get; set; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        //string ContentType { get; set; }
        /// <summary>
        /// Sets a header value.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="value">The value.</param>
        //void SetHeader(string headerName, string value);

        /// <summary>
        /// Sets a cookie.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="expires">The time at which the cookie expires.</param>
        /// <param name="httpOnly">Set to <c>true</c> to prevent cookie being read by client-side script.</param>
        /// <param name="secure">Set to <c>true</c> to transmit cookie using SSL.</param>
        /// <param name="domain">The domain with which to associate the cookie.</param>
        /// <param name="path">The virtual path with which to associate the cookie.</param>
        void SetCookie(string name, string value, DateTime? expires = null, bool httpOnly = false, bool secure = false, string domain = null, string path = null);

        /// <summary>
        /// Sets a multi-value cookie.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="values">The key/value pairs contained in the cookie.</param>
        /// <param name="expires">The time at which the cookie expires.</param>
        /// <param name="httpOnly">Set to <c>true</c> to prevent cookie being read by client-side script.</param>
        /// <param name="secure">Set to <c>true</c> to transmit cookie using SSL.</param>
        /// <param name="domain">The domain with which to associate the cookie.</param>
        /// <param name="path">The virtual path with which to associate the cookie.</param>
        void SetCookie(string name, IDictionary<string,string> values, DateTime? expires = null, bool httpOnly = false, bool secure = false, string domain = null, string path = null);

        /// <summary>
        /// Removes the named cookie.
        /// </summary>
        /// <param name="name">The name of the cookie to remove.</param>
        void RemoveCookie(string name);

        /// <summary>
        /// Transmits a file to the client using whatever optimizations are available.
        /// </summary>
        /// <param name="file">The file path.</param>
        //void TransmitFile(string file);
        /// <summary>
        /// Disables caching of the resource.
        /// </summary>
        void DisableCache();
        /// <summary>
        /// Sets the cache absolute expiry.
        /// </summary>
        /// <param name="expiresAt">The expires at.</param>
        void SetCacheAbsoluteExpiry(DateTime expiresAt);
        /// <summary>
        /// Sets the cache sliding expiry.
        /// </summary>
        /// <param name="expiresIn">The expires in.</param>
        void SetCacheSlidingExpiry(TimeSpan expiresIn);
        /// <summary>
        /// Sets the ETag.
        /// </summary>
        /// <param name="etag">The ETag.</param>
        void SetETag(string etag);
        /// <summary>
        /// Sets the last modified date.
        /// </summary>
        /// <param name="lastModified">The last modified date.</param>
        void SetLastModified(DateTime lastModified);
        /// <summary>
        /// Sets the cache vary by content encodings.
        /// </summary>
        /// <param name="varyByContentEncodings">The vary by content encodings.</param>
        void SetCacheVaryByContentEncodings(ICollection<string> varyByContentEncodings);
        /// <summary>
        /// Sets the cache vary by params.
        /// </summary>
        /// <param name="varyByParams">The vary by params.</param>
        void SetCacheVaryByParams(ICollection<string> varyByParams);
        /// <summary>
        /// Sets the cache vary by headers.
        /// </summary>
        /// <param name="varyByHeaders">The vary by headers.</param>
        void SetCacheVaryByHeaders(ICollection<string> varyByHeaders);
    }
}