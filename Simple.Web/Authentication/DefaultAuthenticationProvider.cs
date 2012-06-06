namespace Simple.Web.Authentication
{
    using System;
    using Simple.Web.Http;

    /// <summary>
    /// Default implementation of <see cref="IAuthenticationProvider"/>. Not for production use.
    /// </summary>
    public class DefaultAuthenticationProvider : IAuthenticationProvider
    {
        private const string UserCookieName = "A2A4CFE430BF42C7BDCB1E16571BA946";

        /// <summary>
        /// Gets the logged in user from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A user, or <c>null</c> if no user is authenticated.
        /// </returns>
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

        /// <summary>
        /// Sets the logged in user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="user">The user.</param>
        public void SetLoggedInUser(IContext context, IUser user)
        {
            context.Response.SetCookie(UserCookieName, user.Guid.ToString("N"));
        }
    }
}