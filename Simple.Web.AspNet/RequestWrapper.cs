namespace Simple.Web.AspNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Web;

    internal class RequestWrapper : IRequest
    {
        private readonly HttpRequest _httpRequest;
        private IHeaderCollection _headers;
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

        public ICookieCollection Cookies
        {
            get { return _cookies ?? (_cookies = new CookieWrapperCollection(_httpRequest.Cookies)); }
        }
    }

    internal class CookieWrapperCollection : ICookieCollection
    {
        private readonly HttpCookieCollection _cookies;

        public CookieWrapperCollection(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }

        public IEnumerator<ICookie> GetEnumerator()
        {
            return _cookies.AllKeys.Select(k => new CookieWrapper(_cookies[k])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ICookie item)
        {
            var cookie = item as CookieWrapper;
            if (cookie == null) throw new InvalidOperationException();
            _cookies.Add(cookie.HttpCookie);
        }

        public void Clear()
        {
            _cookies.Clear();
        }

        public bool Contains(ICookie item)
        {
            var cookie = item as CookieWrapper;
            if (cookie == null) return false;
            return _cookies.AllKeys.Contains(cookie.Name);
        }

        public void CopyTo(ICookie[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(ICookie item)
        {
            if (_cookies.AllKeys.Contains(item.Name)) return false;
            _cookies.Remove(item.Name);
            return true;
        }

        public int Count
        {
            get { return _cookies.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICookie New(string name)
        {
            return new CookieWrapper(new HttpCookie(name));
        }
    }
}