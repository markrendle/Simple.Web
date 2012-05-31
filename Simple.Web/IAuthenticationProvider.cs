namespace Simple.Web
{
    using System;
    using Http;

    public interface IAuthenticationProvider
    {
        IUser GetLoggedInUser(IContext context);
        void SetLoggedInUser(IContext context, IUser user);
    }

    public class DefaultAuthenticationProvider : IAuthenticationProvider
    {
        private const string UserCookieName = "A2A4CFE430BF42C7BDCB1E16571BA946";

        public IUser GetLoggedInUser(IContext context)
        {
            Guid userGuid;
            var cookie = context.Request.Cookies[UserCookieName];
            if (cookie != null && (!string.IsNullOrWhiteSpace(cookie.Value)) && Guid.TryParse(cookie.Value, out userGuid))
            {
                return new User(userGuid, string.Empty);
            }
            return AnonymousUser.Instance;
        }

        public void SetLoggedInUser(IContext context, IUser user)
        {
            context.Response.Cookies[UserCookieName].Value = user.Guid.ToString("N");
        }
    }

    class AnonymousUser : IUser
    {
        public static readonly IUser Instance = new AnonymousUser();
        private AnonymousUser()
        {
            
        }

        public Guid Guid
        {
            get { return Guid.Empty; }
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