using Simple.Web.Authentication;
using Simple.Web.Http;

namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.DependencyInjection;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class OptionalAuthentication
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <param name="container">The scoped container.</param>
        /// <returns><c>true</c></returns>
        public static bool Impl(IOptionalAuthentication handler, IContext context, ISimpleContainerScope container)
        {
            var authenticationProvider = SimpleWeb.Configuration.AuthenticationProvider ?? new DefaultAuthenticationProvider();
            handler.CurrentUser = authenticationProvider.GetLoggedInUser(context);
            return true;
        }
    }
}