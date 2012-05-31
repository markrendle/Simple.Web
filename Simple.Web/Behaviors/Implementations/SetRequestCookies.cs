namespace Simple.Web.Behaviors.Implementations
{
    using Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetRequestCookies
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(IReadCookies setCookies, IContext context)
        {
            setCookies.RequestCookies = context.Request.Cookies;
        }
    }
}
