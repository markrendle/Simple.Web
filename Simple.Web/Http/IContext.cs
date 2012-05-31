namespace Simple.Web.Http
{
    using Authentication;

    /// <summary>
    /// Represents the context for a request/response cycle.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        IResponse Response { get; }
    }
}