namespace Simple.Web.CodeGeneration
{
    using System;

    /// <summary>
    /// A scoped wrapper around a Handler for use by Dependency Injection libraries.
    /// </summary>
    public interface IScopedHandler : IDisposable
    {
        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        object Handler { get; }
    }
}