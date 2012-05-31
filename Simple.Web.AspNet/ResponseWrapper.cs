namespace Simple.Web.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Http;

    internal class ResponseWrapper : IResponse
    {
        private readonly HttpResponse _httpResponse;
        private ICookieCollection _cookies;

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

        public void DisableCache()
        {
            _httpResponse.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            _httpResponse.Cache.SetValidUntilExpires(false);
            _httpResponse.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            _httpResponse.Cache.SetCacheability(HttpCacheability.NoCache);
            _httpResponse.Cache.SetNoStore(); 
        }

        public void SetCacheAbsoluteExpiry(DateTime expiresAt)
        {
            _httpResponse.Cache.SetExpires(expiresAt);
        }

        public void SetCacheSlidingExpiry(TimeSpan expiresIn)
        {
            _httpResponse.Cache.SetSlidingExpiration(true);
            _httpResponse.Cache.SetExpires(DateTime.Now + expiresIn);
        }

        public void SetETag(string etag)
        {
            _httpResponse.Cache.SetETag(etag);
        }

        public void SetLastModified(DateTime lastModified)
        {
            _httpResponse.Cache.SetLastModified(lastModified);
        }

        public void SetCacheVaryByContentEncodings(ICollection<string> varyByContentEncodings)
        {
            foreach (var encoding in varyByContentEncodings)
            {
                _httpResponse.Cache.VaryByContentEncodings[encoding] = true;
            }
        }

        public void SetCacheVaryByParams(ICollection<string> varyByParams)
        {
            if (varyByParams.Count == 1 && "none".Equals(varyByParams.Single(), StringComparison.OrdinalIgnoreCase))
            {
                _httpResponse.Cache.VaryByParams.IgnoreParams = true;
                return;
            }
            foreach (var param in varyByParams)
            {
                _httpResponse.Cache.VaryByParams[param] = true;
            }
        }

        public void SetCacheVaryByHeaders(ICollection<string> varyByHeaders)
        {
            foreach (var header in varyByHeaders)
            {
                switch (header.ToLowerInvariant())
                {
                    case "accept":
                        _httpResponse.Cache.VaryByHeaders.AcceptTypes = true;
                        break;
                    case "accept-charset":
                        _httpResponse.Cache.VaryByHeaders.UserCharSet = true;
                        break;
                    case "accept-language":
                        _httpResponse.Cache.VaryByHeaders.UserLanguage = true;
                        break;
                    case "user-agent":
                        _httpResponse.Cache.VaryByHeaders.UserAgent = true;
                        break;
                    default:
                        _httpResponse.Cache.VaryByHeaders[header] = true;
                        break;
                }
            }
        }

        public void SetHeader(string headerName, string value)
        {
            _httpResponse.Headers.Set(headerName, value);
        }

        public ICookieCollection Cookies
        {
            get { return _cookies ?? (_cookies = new CookieWrapperCollection(_httpResponse.Cookies)); }
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