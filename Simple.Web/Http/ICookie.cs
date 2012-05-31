namespace Simple.Web.Http
{
    using System;

    public interface ICookie
    {
        string Name { get; set; }
        bool Secure { get; set; }
        bool HttpOnly { get; set; }
        DateTime Expires { get; set; }
        string Value { get; set; }
        string this[string key] { get; set; }
    }
}