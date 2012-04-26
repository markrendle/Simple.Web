namespace Simple.Web
{
    public interface ISetCookies
    {
        ICookieCollection ResponseCookies { set; }
    }

    public interface IReadCookies
    {
        ICookieCollection RequestCookies { set; }
    }
}