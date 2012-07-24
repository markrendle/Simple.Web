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
        /// <returns></returns>
        public static bool Impl(IMayRedirect handler, IContext context)
        {
            int code;
            if (int.TryParse(context.Response.Status.Substring(0, 3), NumberStyles.Integer, CultureInfo.InvariantCulture, out code))
            {
                if ((code >= 301 && code <= 303) || code == 307)
                {
                    context.Response.SetHeader("Location", handler.Location);
                    context.Response.SetCookie("Test", "Cookie");
                    return false;
                        // this cancels the responder task, so doesn't require a view. Cookie task MUST come before this!
                }
            }
            return true;
        }
    }
}
