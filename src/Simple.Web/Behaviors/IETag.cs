namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Indicates that the resource for a handler has an ETag.
    /// </summary>
    [RequestBehavior(typeof(Implementations.SetInputETag))]
    [ResponseBehavior(typeof(Implementations.SetOutputETag))]
    public interface IETag
    {
        /// <summary>
        /// Used by the framework to set the ETag from the Request.
        /// </summary>
        /// <value>
        /// The input ETag.
        /// </value>
        string InputETag { set; }

        /// <summary>
        /// The ETag to include as a header in the Response.
        /// </summary>
        string OutputETag { get; }
    }
}