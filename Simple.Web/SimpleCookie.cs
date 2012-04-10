namespace Simple.Web
{
    using System;
    using System.Web;

    public class SimpleCookie : ICookie
    {
        private readonly HttpCookie _cookie;
        public string Name
        {
            get { return _cookie.Name; }
            set { _cookie.Name = value; }
        }

        public bool Secure
        {
            get { return _cookie.Secure; }
            set { _cookie.Secure = value; }
        }

        public bool HttpOnly
        {
            get { return _cookie.HttpOnly; }
            set { _cookie.HttpOnly = value; }
        }

        public DateTime Expires
        {
            get { return _cookie.Expires; }
            set { _cookie.Expires = value; }
        }

        public string Value
        {
            get { return _cookie.Value; }
            set { _cookie.Value = value; }
        }

        public string this[string key]
        {
            get { return _cookie[key]; }
            set { _cookie[key] = value; }
        }

        public HttpCookie HttpCookie
        {
            get { return _cookie; }
        }

        public SimpleCookie(string name, string value) : this(new HttpCookie(name, value))
        {
        }

        public SimpleCookie(string name) : this(new HttpCookie(name))
        {
        }

        public SimpleCookie(HttpCookie cookie)
        {
            _cookie = cookie;
        }
    }
}