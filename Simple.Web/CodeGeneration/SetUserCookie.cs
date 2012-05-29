namespace Simple.Web.CodeGeneration
{
    static class SetUserCookie
    {
        internal static void Impl(ILogin login, IContext context)
        {
            if (login.LoggedInUser != null)
            {
                SimpleWeb.Configuration.AuthenticationProvider.SetLoggedInUser(context, login.LoggedInUser);
            }
        }
    }
}