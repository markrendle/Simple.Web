namespace Simple.Web
{
    public interface IAuthenticationProvider
    {
        IUser GetLoggedInUser(IContext context);
    }

    public class AuthenticationProvider : IAuthenticationProvider
    {
        public IUser GetLoggedInUser(IContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}