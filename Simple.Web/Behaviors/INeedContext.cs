namespace Simple.Web.Behaviors
{
    using System;
    using Http;

    /// <summary>
    /// Represents a handler which requires access to the underlying <see cref="IContext"/> for the operation.
    /// </summary>
    [RequestBehavior(typeof(Implementations.SetContext))]
    [Obsolete("If you need something from the context, consider implementing your own behavior.")]
    public interface INeedContext
    {
        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        /// <remarks>
        /// This property will be set by the framework.
        /// </remarks>
        IContext Context { set; }
    }
}