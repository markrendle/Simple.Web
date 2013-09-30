namespace Simple.Web.Behaviors
{
    using Simple.Web.Behaviors.Implementations;

    /// <summary>
    /// Indicates that the resource for a handler has an ETag.
    /// </summary>
    [RequestBehavior(typeof(SetInputETag))]
    [ResponseBehavior(typeof(SetOutputETag))]
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