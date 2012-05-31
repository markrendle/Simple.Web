namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Represents the handler which serves the Login Page for a Forms-based authentication application.
    /// </summary>
    public interface ILoginPage
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
        string ReturnUrl { set; }
    }
}
