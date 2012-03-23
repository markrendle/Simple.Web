namespace Simple.Web
{
    /// <summary>
    /// Represents an endpoint that may respond with a 301, 302, 303 or 307 Redirect status.
    /// </summary>
    public interface IRedirect
    {
        /// <summary>
        /// Gets the Location value to be added to the Response header.
        /// </summary>
        string Location { get; }
    }
}