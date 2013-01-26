namespace Simple.Web.Authentication
{
    using Simple.Web.Http;

    /// <summary>
    /// Provides methods for supplying user information from a request context.
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Gets the logged in user from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A user, or <c>null</c> if no user is authenticated.</returns>
        IUser GetLoggedInUser(IContext context);

        /// <summary>
        /// Sets the logged in user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="user">The user.</param>
        void SetLoggedInUser(IContext context, IUser user);
    }
}