using Simple.Web.Authentication;

namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Indicates that a handler resource behaves differently for authenticated users.
    /// </summary>
    /// <remarks>If a user is authenticated, the <see cref="CurrentUser"/> property will be set.
    /// Either way, the resource will still be loaded.</remarks>
    [RequestBehavior(typeof(Implementations.OptionalAuthentication))]
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