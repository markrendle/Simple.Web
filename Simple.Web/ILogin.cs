namespace Simple.Web
{
    /// <summary>
    /// Represents the handler which processes the login for a Forms-based authentication application.
    /// </summary>
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