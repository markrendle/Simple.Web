namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;

    internal class RequestWrapper : IRequest
    {
        private readonly HttpRequest _httpRequest;
        private IHeaderCollection _headers;

        public RequestWrapper(HttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public Uri Url
        {
            get { return _httpRequest.Url; }
        }

        public IList<string> AcceptTypes
        {
            get { return _httpRequest.AcceptTypes; }
        }

        public NameValueCollection QueryString
        {
            get { return _httpRequest.QueryString; }
        }

        public Stream InputStream
        {
            get { return _httpRequest.InputStream; }
        }

        public string ContentType
        {
            get { return _httpRequest.ContentType; }
        }

        public string HttpMethod
        {
            get { return _httpRequest.HttpMethod; }
        }

        public IHeaderCollection Headers
        {
            get { return _headers ?? (_headers =  new HeaderCollection(_httpRequest.Headers)); }
        }

        public IEnumerable<IPostedFile> Files
        {
            get
            {
                for (int i = 0; i < _httpRequest.Files.Count; i++)
                {
                    yield return new PostedFile(_httpRequest.Files.Get(i));
                }
            }
        }
    }
}