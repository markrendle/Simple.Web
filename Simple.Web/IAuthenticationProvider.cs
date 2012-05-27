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
            return AnonymousUser.Instance;
        }
    }

    class AnonymousUser : IUser
    {
        public static readonly IUser Instance = new AnonymousUser();
        private AnonymousUser()
        {
            
        }
        public string Name
        {
            get { return "Anonymous"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }
    }
}