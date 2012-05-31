namespace Simple.Web.Http
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public interface IHeaderCollection
    {
        int Count { get; }
        IEnumerable<string> Keys { get; }
        void Add(NameValueCollection c);
        void Add(string name, string value);
        string Get(string name);
        string[] GetValues(string name);
        void Set(string name, string value);
        void Remove(string name);
        string Get(int index);
        string[] GetValues(int index);
        string GetKey(int index);
        string this[string name] { get; set; }
        string this[int index] { get; }
        bool HasKeys();
        void Clear();
    }
}