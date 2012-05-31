namespace Simple.Web.Behaviors
{
    using Http;

    [RequestBehavior(typeof(Implementations.SetRequestCookies))]
    public interface IReadCookies
    {
        ICookieCollection RequestCookies { set; }
    }
}