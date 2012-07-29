namespace Simple.Web.Behaviors.Implementations
{
    using System.Globalization;
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
        /// <returns><c>false</c> (to prevent response output) if the status is a redirect code; otherwise, <c>true</c>.</returns>
        public static bool Impl(IMayRedirect handler, IContext context)
        {
            int code = context.Response.Status.Code;
            if ((code >= 301 && code <= 303) || code == 307)
            {
                context.Response.SetHeader("Location", handler.Location);
                context.Response.SetCookie("Test", "Cookie");
                return false;
                // this cancels the responder task, so doesn't require a view. Cookie task MUST come before this!
            }
            return true;
        }
    }
}
