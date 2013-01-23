namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Represents an handler that may respond with a 301, 302, 303 or 307 Redirect status.
    /// </summary>
    [ResponseBehavior(typeof(Implementations.Redirect), Priority = Priority.High)]
    public interface IMayRedirect
    {
        /// <summary>
        /// Gets the Location value to be added to the Response header.
        /// </summary>
        /// <remarks>
        /// This property must not be <c>null</c> if handler returns a 30x Redirect status.
        /// </remarks>
        string Location { get; }
    }
}