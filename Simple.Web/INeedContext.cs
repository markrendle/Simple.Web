namespace Simple.Web
{
    /// <summary>
    /// Represents a handler which requires access to the underlying <see cref="IContext"/> for the operation.
    /// </summary>
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