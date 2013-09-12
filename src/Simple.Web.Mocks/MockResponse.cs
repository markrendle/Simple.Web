using System.Runtime.Remoting.Messaging;

namespace Simple.Web.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Http;

    public class MockResponse : IResponse
    {
        public MockResponse()
        {
            Headers = new Dictionary<string, string[]>();
        }
        public Status Status { get; set; }
        public Func<Stream, Task> WriteFunction { get; set; }
        public IDictionary<string, string[]> Headers { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public TextWriter Output { get; set; }


        public string ContentType { get; set; }

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
    }
}