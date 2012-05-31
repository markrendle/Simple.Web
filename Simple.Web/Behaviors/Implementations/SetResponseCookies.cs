namespace Simple.Web.Behaviors.Implementations
{
    using Http;

    public static class SetResponseCookies
    {
        public static void Impl(ISetCookies setCookies, IContext context)
        {
            if (context.Request != null)
            {
                setCookies.ResponseCookies = context.Request.Cookies;
            }
        }
    }
}