namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class SetUserCookie
    {
        public static void Impl(ILogin login, IContext context)
        {
            if (login.LoggedInUser != null)
            {
                SimpleWeb.Configuration.AuthenticationProvider.SetLoggedInUser(context, login.LoggedInUser);
            }
        }
    }
}