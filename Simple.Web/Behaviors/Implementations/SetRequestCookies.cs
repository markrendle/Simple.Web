namespace Simple.Web.Behaviors.Implementations
{
    using Http;

    public static class SetRequestCookies
    {
        public static void Impl(IReadCookies setCookies, IContext context)
        {
            setCookies.RequestCookies = context.Request.Cookies;
        }
    }
}
