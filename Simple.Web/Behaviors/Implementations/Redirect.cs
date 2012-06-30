namespace Simple.Web.Behaviors.Implementations
{
    using Behaviors;
    using Http;

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
        public static bool Impl(IMayRedirect handler, IContext context)
        {
            if ((context.Response.StatusCode >= 301 && context.Response.StatusCode <= 303) || context.Response.StatusCode == 307)
            {
                context.Response.SetHeader("Location", handler.Location);
				context.Response.SetCookie("Test", "Cookie");
                return false; // this cancels the responder task, so doesn't require a view. Cookie task MUST come before this!
            }
            return true;
        }
    }
}
