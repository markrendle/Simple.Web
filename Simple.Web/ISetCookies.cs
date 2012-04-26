namespace Simple.Web
{
    using System.Collections.Generic;

    public interface ISetCookies
    {
        ICookieCollection ResponseCookies { get;  set; }
    }

    public interface IReadCookies
    {
        ICookieCollection RequestCookies { get; set; }
    }
}