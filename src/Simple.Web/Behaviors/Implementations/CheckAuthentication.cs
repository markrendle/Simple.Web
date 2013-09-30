namespace Simple.Web.Behaviors.Implementations
{
    using System;
    using System.Linq;

    using Simple.Web.Authentication;
    using Simple.Web.Http;
    using Simple.Web.MediaTypeHandling;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class CheckAuthentication
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</returns>
        public static bool Impl(IRequireAuthentication handler, IContext context)
        {
            var authenticationProvider = SimpleWeb.Configuration.AuthenticationProvider ?? new DefaultAuthenticationProvider();
            var user = authenticationProvider.GetLoggedInUser(context);
            if (user == null || !user.IsAuthenticated)
            {
                Redirect(context);
                return false;
            }

            handler.CurrentUser = user;
            return true;
        }

        internal static void Redirect(IContext context)
        {
            var accept = context.Request.GetAccept();
            if (accept.Contains(MediaType.Html) || accept.Contains(MediaType.XHtml))
            {
                if (SimpleWeb.Configuration.LoginPage != null)
                {
                    var uriTemplateAttribute = UriTemplateAttribute.Get(SimpleWeb.Configuration.LoginPage).FirstOrDefault();
                    if (uriTemplateAttribute != null)
                    {
                        var redirect = uriTemplateAttribute.Template + "?returnUrl=" + Uri.EscapeDataString(context.Request.Url.ToString());
                        context.Response.SetHeader("Location", redirect);
                        context.Response.Status = Status.TemporaryRedirect(redirect);
                        return;
                    }
                }
            }
            context.Response.Status = "401 Unauthorized";
        }
    }
}