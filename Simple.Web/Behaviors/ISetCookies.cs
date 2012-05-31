namespace Simple.Web.Behaviors
{
    using Http;

    [RequestBehavior(typeof(Implementations.SetResponseCookies))]
    public interface ISetCookies
    {
        ICookieCollection ResponseCookies { set; }
    }
}