using System;

namespace Simple.Web.CodeGeneration
{
    using Simple.Web.DependencyInjection;

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

        /// <summary>
        /// Gets the current scoped container.
        /// </summary>
        /// <value>
        /// The container scope.
        /// </value>
        ISimpleContainerScope Container { get; }
    }
}
