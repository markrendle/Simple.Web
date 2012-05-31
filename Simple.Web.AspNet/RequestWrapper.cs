namespace Simple.Web.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Helpers;
    using Http;

    internal class RequestWrapper : IRequest
    {
        private readonly HttpRequest _httpRequest;
        private ICookieCollection _cookies;

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

        public ILookup<string, string> QueryString
        {
            get { return _httpRequest.QueryString.ToLookup(); }
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

        public ILookup<string, string> Headers
        {
            get
            {
                return _httpRequest.Headers.ToLookup();
            }
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

        public ICookieCollection Cookies
        {
            get { return _cookies ?? (_cookies = new CookieWrapperCollection(_httpRequest.Cookies)); }
        }
    }
}