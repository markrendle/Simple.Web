namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
        int StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        /// <value>
        /// The status description.
        /// </value>
        string StatusDescription { get; set; }
        /// <summary>
        /// Gets the output stream.
        /// </summary>
        Stream OutputStream { get; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        string ContentType { get; set; }
        /// <summary>
        /// Sets a header value.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="value">The value.</param>
        void SetHeader(string headerName, string value);
        /// <summary>
        /// Gets the cookies.
        /// </summary>
        ICookieCollection Cookies { get; }
        /// <summary>
        /// Transmits a file to the client using whatever optimizations are available.
        /// </summary>
        /// <param name="file">The file path.</param>
        void TransmitFile(string file);
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