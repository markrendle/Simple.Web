namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetUserCookie
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(ILogin login, IContext context)
        {
            if (login.LoggedInUser != null)
            {
                SimpleWeb.Configuration.AuthenticationProvider.SetLoggedInUser(context, login.LoggedInUser);
            }
        }
    }
}