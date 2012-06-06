namespace Simple.Web.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Http;

    public class MockResponse : IResponse
    {
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public TextWriter Output { get; set; }

        public Stream OutputStream { get; set; }

        public string ContentType { get; set; }

        public void SetHeader(string headerName, string value)
        {
        }

        public void SetCookie(string name, string value, DateTime? expires, bool httpOnly = false, bool secure = false, string domain = null, string path = null)
        {
        }

        public void SetCookie(string name, IDictionary<string, string> values, DateTime? expires, bool httpOnly = false, bool secure = false, string domain = null, string path = null)
        {
        }

        public ICookieCollection Cookies
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
        }

        public void Flush()
        {
        }

        public void Write(string s)
        {
            Write(Encoding.UTF8.GetBytes(s));
        }

        public void Write(object obj)
        {
            if (obj != null) Write(obj.ToString());
        }

        public void Write(char[] buffer, int index, int count)
        {
            Write(Encoding.UTF8.GetBytes(buffer));
        }

        public void Write(char ch)
        {
            Write(Encoding.UTF8.GetBytes(new[] { ch }));
        }

        public void TransmitFile(string file)
        {
        }

        public void SetCookie(ICookie cookie)
        {
        }

        public void DisableCache()
        {
        }

        public void SetCacheAbsoluteExpiry(DateTime expiresAt)
        {
        }

        public void SetCacheSlidingExpiry(TimeSpan expiresIn)
        {
        }

        public void SetETag(string etag)
        {
        }

        public void SetLastModified(DateTime lastModified)
        {
        }

        public void SetCacheVaryByContentEncodings(ICollection<string> varyByContentEncodings)
        {
        }

        public void SetCacheVaryByParams(ICollection<string> varyByParams)
        {
        }

        public void SetCacheVaryByHeaders(ICollection<string> varyByHeaders)
        {
        }

        private void Write(byte[] bytes)
        {
            OutputStream.Write(bytes, 0, bytes.Length);
        }
    }
}