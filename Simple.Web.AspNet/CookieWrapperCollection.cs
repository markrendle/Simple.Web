namespace Simple.Web.AspNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    internal class CookieWrapperCollection : ICookieCollection
    {
        private readonly HttpCookieCollection _cookies;

        public IEnumerable<string> Keys 
        {
            get { return _cookies.AllKeys.AsEnumerable(); }
        }

        public ICookie this[int index]
        {
            get { return new CookieWrapper(_cookies[index]); }
        }

        public ICookie this[string name]
        {
            get { return new CookieWrapper(_cookies[name]); }
        }

        public string GetKey(int index)
        {
            return _cookies.GetKey(index);
        }

        public ICookie Get(int index)
        {
            return new CookieWrapper(_cookies.Get(index));
        }

        public ICookie Get(string name)
        {
            return new CookieWrapper(_cookies.Get(name));
        }

        public void Set(ICookie cookie)
        {
            var wrapper = cookie as CookieWrapper;
            if (wrapper == null) throw new InvalidOperationException();
            _cookies.Set(wrapper.HttpCookie);
        }

        public void Remove(string name)
        {
            _cookies.Remove(name);
        }

        public void Clear()
        {
            _cookies.Clear();
        }

        public void Add(ICookie cookie)
        {
            var wrapper = cookie as CookieWrapper;
            if (wrapper == null) throw new InvalidOperationException();
            _cookies.Add(wrapper.HttpCookie);
        }

        public ICookie New(string name)
        {
            return new CookieWrapper(new HttpCookie(name));
        }

        public int Count
        {
            get { return _cookies.Count; }
        }

        internal CookieWrapperCollection(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }
    }
}