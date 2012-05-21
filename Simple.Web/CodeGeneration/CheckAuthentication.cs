namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Linq;
    using Helpers;

    internal static class CheckAuthentication
    {
        internal static bool Impl(IRequireAuthentication handler, IContext context)
        {
            var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ?? new AuthenticationProvider();
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
                                                     Uri.EscapeUriString(context.Request.Url.ToString()));
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