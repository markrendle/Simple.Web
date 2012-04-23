namespace Simple.Web
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;

    internal class ResponseWrapper : IResponse
    {
        private readonly HttpResponse _httpResponse;
        private IHeaderCollection _headers;

        public void Write(string s)
        {
            _httpResponse.Write(s);
        }

        public void Write(object obj)
        {
            _httpResponse.Write(obj);
        }

        public void Write(char[] buffer, int index, int count)
        {
            _httpResponse.Write(buffer, index, count);
        }

        public ResponseWrapper(HttpResponse httpResponse)
        {
            _httpResponse = httpResponse;
        }

        public int StatusCode
        {
            get { return _httpResponse.StatusCode; }
            set { _httpResponse.StatusCode = value; }
        }

        public string StatusDescription
        {
            get { return _httpResponse.StatusDescription; }
            set { _httpResponse.StatusDescription = value; }
        }

        public TextWriter Output
        {
            get { return _httpResponse.Output; }
        }

        public Stream OutputStream
        {
            get { return _httpResponse.OutputStream; }
        }

        public string ContentType
        {
            get { return _httpResponse.ContentType; }
            set { _httpResponse.ContentType = value; }
        }

        public void Write(char ch)
        {
            _httpResponse.Write(ch);
        }

        public void TransmitFile(string file)
        {
            _httpResponse.TransmitFile(file);
        }

        public void SetCookie(ICookie cookie)
        {
            var simpleCookie = cookie as SimpleCookie;
            if (simpleCookie != null)
            {
                _httpResponse.Cookies.Add(simpleCookie.HttpCookie);
            }
        }

        public void DisableCache()
        {
            _httpResponse.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            _httpResponse.Cache.SetValidUntilExpires(false);
            _httpResponse.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            _httpResponse.Cache.SetCacheability(HttpCacheability.NoCache);
            _httpResponse.Cache.SetNoStore(); 
        }

        public IHeaderCollection Headers
        {
            get { return _headers ?? (_headers =  new HeaderCollection(_httpResponse.Headers)); }
        }

        public void Close()
        {
            _httpResponse.Close();
        }

        public void Flush()
        {
            _httpResponse.Flush();
        }
    }
}