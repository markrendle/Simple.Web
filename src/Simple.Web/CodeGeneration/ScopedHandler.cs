namespace Simple.Web.CodeGeneration
{
    using Simple.Web.DependencyInjection;

    /// <summary>
    /// Default implementation of <see cref="IScopedHandler"/>.
    /// </summary>
    public sealed class ScopedHandler : IScopedHandler
    {
        private readonly ISimpleContainerScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedHandler" /> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="scope">The scope.</param>
        public ScopedHandler(object handler, ISimpleContainerScope scope)
        {
            Handler = handler;
            _scope = scope;
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public object Handler { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _scope.Dispose();
        }

        /// <summary>
        /// Creates the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>An <see cref="IScopedHandler"/> wrapped around a handler object.</returns>
        public static IScopedHandler Create(object handler, ISimpleContainerScope scope)
        {
            return new ScopedHandler(handler, scope);
        }
    }
}