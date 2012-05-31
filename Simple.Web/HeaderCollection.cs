namespace Simple.Web
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Http;

    public class HeaderCollection : IHeaderCollection
    {
        private readonly NameValueCollection _nameValueCollection;

        public HeaderCollection(NameValueCollection nameValueCollection)
        {
            _nameValueCollection = nameValueCollection;
        }

        public int Count
        {
            get { return _nameValueCollection.Count; }
        }

        public IEnumerable<string> Keys
        {
            get { return _nameValueCollection.AllKeys; }
        }

        public void Add(NameValueCollection c)
        {
            _nameValueCollection.Add(c);
        }

        public void Add(string name, string value)
        {
            _nameValueCollection.Add(name, value);
        }

        public string Get(string name)
        {
            return _nameValueCollection.Get(name);
        }

        public string[] GetValues(string name)
        {
            return _nameValueCollection.GetValues(name);
        }

        public void Set(string name, string value)
        {
            _nameValueCollection.Set(name, value);
        }

        public void Remove(string name)
        {
            _nameValueCollection.Remove(name);
        }

        public string Get(int index)
        {
            return _nameValueCollection.Get(index);
        }

        public string[] GetValues(int index)
        {
            return _nameValueCollection.GetValues(index);
        }

        public string GetKey(int index)
        {
            return _nameValueCollection.GetKey(index);
        }

        public string this[string name]
        {
            get { return _nameValueCollection[name]; }
            set { _nameValueCollection[name] = value; }
        }

        public string this[int index]
        {
            get { return _nameValueCollection[index]; }
        }

        public string[] AllKeys
        {
            get { return _nameValueCollection.AllKeys; }
        }

        public bool HasKeys()
        {
            return _nameValueCollection.HasKeys();
        }

        public void Clear()
        {
            _nameValueCollection.Clear();
        }
    }
}