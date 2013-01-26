namespace Simple.Web.Hosting
{
    using System;
    using System.Collections.Concurrent;
    using Simple.Web.CodeGeneration;
    using Simple.Web.Http;

    /// <summary>
    /// Factory class for building runners.
    /// </summary>
    internal class HandlerRunnerFactory
    {
        private readonly ConcurrentDictionary<Type, Action<object, IContext>> _cache =
            new ConcurrentDictionary<Type, Action<object, IContext>>();

        private readonly ConcurrentDictionary<Type, AsyncRunner> _asyncCache =
            new ConcurrentDictionary<Type, AsyncRunner>();

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static readonly HandlerRunnerFactory Instance = new HandlerRunnerFactory();

        private HandlerRunnerFactory()
        {
        }

        /// <summary>
        /// Gets the synchronous runner for the specified handler type.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns></returns>
        public Action<object, IContext> Get(Type handlerType, string httpMethod)
        {
            return _cache.GetOrAdd(handlerType, t => new HandlerRunnerBuilder(t, httpMethod).BuildRunner());
        }

        /// <summary>
        /// Gets the asynchronous runner for the specified handler type.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns></returns>
        public AsyncRunner GetAsync(Type handlerType, string httpMethod)
        {
            return _asyncCache.GetOrAdd(handlerType, t => new HandlerRunnerBuilder(t, httpMethod).BuildAsyncRunner());
        }
    }
}
