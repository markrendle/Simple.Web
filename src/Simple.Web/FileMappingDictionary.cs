namespace Simple.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class FileMappingDictionary : IDictionary<string, PublicFile>
    {
        private readonly IDictionary<string, PublicFile> _dict = new Dictionary<string, PublicFile>(StringComparer.OrdinalIgnoreCase);
        public IEnumerator<KeyValuePair<string, PublicFile>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public void Add(KeyValuePair<string, PublicFile> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<string, PublicFile> item)
        {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, PublicFile>[] array, int arrayIndex)
        {
            _dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, PublicFile> item)
        {
            return _dict.Remove(item);
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dict.IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public void Add(string key, PublicFile value)
        {
            _dict[key] = value;
            if (key.EndsWith("/"))
            {
                key = key.TrimEnd('/');
            }
            else
            {
                key = key + "/";
            }
            if (!_dict.ContainsKey(key))
            {
                _dict.Add(key, value);
            }
        }

        public bool Remove(string key)
        {
            return _dict.Remove(key);
        }

        public bool TryGetValue(string key, out PublicFile value)
        {
            return _dict.TryGetValue(key, out value);
        }

        public PublicFile this[string key]
        {
            get { return _dict[key]; }
            set { _dict[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _dict.Keys; }
        }

        public ICollection<PublicFile> Values
        {
            get { return _dict.Values; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}