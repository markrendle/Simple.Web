using System.Collections.Generic;
using System.Linq;

namespace Simple.Web.AspNet
{
    using System;
    using System.Web;
    using Http;

    public class CookieWrapper : ICookie
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

        public IDictionary<string, string> Values
        {
            get
            {
                return
                    _cookie.Values.AllKeys.Select(s => new KeyValuePair<string, string>(s, _cookie.Values[s])).
                        ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            set
            {
                foreach (var kvp in value)
                {
                    _cookie.Values[kvp.Key] = kvp.Value;
                }
            }
        }

        public string this[string key]
        {
            get { return _cookie[key]; }
            set { _cookie[key] = value; }
        }

        internal HttpCookie HttpCookie
        {
            get { return _cookie; }
        }

        public CookieWrapper(string name, string value) : this(new HttpCookie(name, value))
        {
        }

        public CookieWrapper(string name) : this(new HttpCookie(name))
        {
        }

        public CookieWrapper(HttpCookie cookie)
        {
            _cookie = cookie;
        }

        public static ICookie Wrap(HttpCookie cookie)
        {
            return new CookieWrapper(cookie);
        }

        public static IEnumerable<ICookie> Wrap(HttpCookieCollection collection)
        {
            return collection.Cast<string>().Select(n => Wrap(collection[n]));
        }
    }
}