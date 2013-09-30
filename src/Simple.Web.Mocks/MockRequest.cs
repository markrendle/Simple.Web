namespace Simple.Web.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Simple.Web.Http;

    public class MockRequest : IRequest
    {
        public MockRequest()
        {
            QueryString = new Dictionary<string, string[]>();
            HttpMethod = "GET";
            Host = "localhost";
        }

        public IEnumerable<IPostedFile> Files { get; private set; }

        public IDictionary<string, string[]> Headers { get; set; }

        public string Host { get; private set; }

        public string HttpMethod { get; set; }

        public Stream InputStream { get; set; }

        public IDictionary<string, string[]> QueryString { get; set; }

        public Uri Url { get; set; }
    }
}