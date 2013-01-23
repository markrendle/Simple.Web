using System;

namespace Simple.Web.CodeGeneration
{
    /// <summary>
    /// A scoped wrapper around a Handler for use by Dependency Injection libraries.
    /// </summary>
    public interface IScopedHandler: IDisposable
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
