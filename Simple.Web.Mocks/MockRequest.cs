namespace Simple.Web.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using Http;

    public class MockRequest : IRequest
    {
        public MockRequest()
        {
            QueryString = new Dictionary<string, string[]>();
			HttpMethod = "GET";
        }
        public Uri Url { get; set; }

        public IDictionary<string, string[]> QueryString { get; set; }

        public Stream InputStream { get; set; }

        public string ContentType { get; set; }

        public string HttpMethod { get; set; }

        public IDictionary<string, string[]> Headers { get; set; }

        public IEnumerable<IPostedFile> Files
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, ICookie> Cookies { get; set; }
    }

    public class MockCookie : ICookie
    {
        private readonly Dictionary<string,string> _values = new Dictionary<string, string>();
        public string Name { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public DateTime Expires { get; set; }
        public string Value { get; set; }
        public IDictionary<string, string> Values { get; set; }

        public string this[string key]
        {
            get { return _values.ContainsKey(key) ? _values[key] : null; }
            set { _values[key] = value; }
        }
    }
}