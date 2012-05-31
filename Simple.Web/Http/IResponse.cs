namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IResponse
    {
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        TextWriter Output { get; }
        Stream OutputStream { get; }
        string ContentType { get; set; }
        IHeaderCollection Headers { get; }
        ICookieCollection Cookies { get; }
        void Close();
        void Flush();
        void Write(string s);
        void Write(object obj);
        void Write(char[] buffer, int index, int count);
        void Write(char ch);
        void TransmitFile(string file);
        void DisableCache();
        void SetCacheAbsoluteExpiry(DateTime expiresAt);
        void SetCacheSlidingExpiry(TimeSpan expiresIn);
        void SetETag(string etag);
        void SetLastModified(DateTime lastModified);
        void SetCacheVaryByContentEncodings(ICollection<string> varyByContentEncodings);
        void SetCacheVaryByParams(ICollection<string> varyByParams);
        void SetCacheVaryByHeaders(ICollection<string> varyByHeaders);
    }
}