namespace Simple.Web.Behaviors
{
    using System;

    /// <summary>
    /// Indicates that a handler works with the If-Modified-Since and Last-Modified headers.
    /// </summary>
    [RequestBehavior(typeof(Implementations.SetIfModifiedSince))]
    [ResponseBehavior(typeof(Implementations.SetLastModified))]
    public interface IModified
    {
        /// <summary>
        /// The If-Modified-Since value from the Request header.
        /// </summary>
        /// <value>
        /// If modified since.
        /// </value>
        DateTime? IfModifiedSince { set; }
        /// <summary>
        /// The value for the Last-Modified header in the Response.
        /// </summary>
        DateTime? LastModified { get; }
    }
}