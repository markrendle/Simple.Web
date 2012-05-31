namespace Simple.Web.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using Helpers;
    using Http;

    public class MockRequest : IRequest
    {
        public MockRequest()
        {
            QueryString = new NameValueCollection().ToLookup();
        }
        public Uri Url { get; set; }

        public IList<string> AcceptTypes { get; set; }

        public ILookup<string, string> QueryString { get; set; }

        public Stream InputStream { get; set; }

        public string ContentType { get; set; }

        public string HttpMethod { get; set; }

        public ILookup<string,string> Headers { get; set; }

        public IEnumerable<IPostedFile> Files
        {
            get { throw new NotImplementedException(); }
        }

        public ICookieCollection Cookies
        {
            get { throw new NotImplementedException(); }
        }
    }
}