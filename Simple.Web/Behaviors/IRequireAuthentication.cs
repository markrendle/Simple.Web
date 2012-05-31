namespace Simple.Web.Behaviors
{
    using Authentication;

    /// <summary>
    /// Indicates that a handler resource is only available to authenticated users.
    /// </summary>
    /// <remarks>If a user is authenticated, the <see cref="CurrentUser"/> property will be set.
    /// If not, they will be redirected to the login page.</remarks>
    [RequestBehavior(typeof(Implementations.CheckAuthentication), Priority = Priority.Highest)]
    public interface IRequireAuthentication
    {
        /// <summary>
        /// Used by the framework to set the current user.
        /// </summary>
        /// <value>
        /// The current user.
        /// </value>
        IUser CurrentUser { set; }
    }
}