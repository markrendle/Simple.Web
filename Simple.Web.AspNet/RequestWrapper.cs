namespace Simple.Web.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Helpers;
    using Http;

    internal class RequestWrapper : IRequest
    {
        private readonly HttpRequest _httpRequest;
        private IDictionary<string, ICookie> _cookies;

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

        public IDictionary<string, string[]> QueryString
        {
            get { return _httpRequest.QueryString.ToDictionary(); }
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

        public IDictionary<string, string[]> Headers
        {
            get
            {
                return _httpRequest.Headers.ToDictionary();
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

        public IDictionary<string, ICookie> Cookies
        {
            get { return _cookies ?? (_cookies = CookieWrapper.Wrap(_httpRequest.Cookies).ToDictionary(c => c.Name, c => c)); }
        }
    }

    static class NameValueCollectionEx
    {
        public static IDictionary<string,string[]> ToDictionary(this NameValueCollection collection)
        {
            return collection.AllKeys.ToDictionary(k => k, collection.GetValues);
        }
    }
}