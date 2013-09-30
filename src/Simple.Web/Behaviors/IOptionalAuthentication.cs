namespace Simple.Web.Behaviors
{
    using Simple.Web.Authentication;
    using Simple.Web.Behaviors.Implementations;

    /// <summary>
    /// Indicates that a handler resource behaves differently for authenticated users.
    /// </summary>
    /// <remarks>If a user is authenticated, the <see cref="CurrentUser"/> property will be set.
    /// Either way, the resource will still be loaded.</remarks>
    [RequestBehavior(typeof(OptionalAuthentication))]
    public interface IOptionalAuthentication
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