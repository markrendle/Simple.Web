namespace Simple.Web.Behaviors
{
    using Authentication;

    /// <summary>
    /// Represents the handler which processes the login for a Forms-based authentication application.
    /// </summary>
    [ResponseBehavior(typeof(Implementations.SetUserCookie), Priority = Priority.Highest)]
    public interface ILogin
    {
        /// <summary>
        /// Sets the URL that the application should return to once login is complete.
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        /// <remarks>
        /// This property will be set by the framework.
        /// </remarks>
        IUser LoggedInUser { get; }
    }
}