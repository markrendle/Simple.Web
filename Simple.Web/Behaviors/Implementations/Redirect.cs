namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class Redirect
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static bool Impl(IMayRedirect handler, IContext context, Status status)
        {
            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                context.Response.SetHeader("Location", handler.Location);
                return false;
            }
            return true;
        }
    }
}
