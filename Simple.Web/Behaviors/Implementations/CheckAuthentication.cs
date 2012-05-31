namespace Simple.Web.Behaviors.Implementations
{
    using System;
    using System.Linq;
    using Simple.Web.Behaviors;
    using Simple.Web.Helpers;
    using Simple.Web.Http;

    public static class CheckAuthentication
    {
        public static bool Impl(IRequireAuthentication handler, IContext context)
        {
            var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ?? new DefaultAuthenticationProvider();
            var user = authenticationProvider.GetLoggedInUser(context);
            if (user == null || !user.IsAuthenticated)
            {
                if (SimpleWeb.Configuration.LoginPage != null)
                {
                    var uriTemplateAttribute = UriTemplateAttribute.Get(SimpleWeb.Configuration.LoginPage).FirstOrDefault();
                    if (uriTemplateAttribute != null)
                    {
                        context.Response.Headers.Set("Location",
                                                     uriTemplateAttribute.Template + "?returnUrl=" +
                                                     Uri.EscapeDataString(context.Request.Url.ToString()));
                        context.Response.SetStatus(Status.TemporaryRedirect);
                        return false;
                    }
                }
                context.Response.StatusCode = 401;
                context.Response.StatusDescription = "Unauthorized";
                return false;
            }

            handler.CurrentUser = user;
            return true;
        }
    }
}