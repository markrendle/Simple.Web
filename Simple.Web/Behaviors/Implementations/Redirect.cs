namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class Redirect
    {
        public static bool Impl(IMayRedirect handler, IContext context, Status status)
        {
            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                context.Response.Headers.Set("Location", handler.Location);
                return false;
            }
            return true;
        }
    }
}
