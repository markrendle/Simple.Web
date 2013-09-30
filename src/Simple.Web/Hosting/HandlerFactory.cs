namespace Simple.Web.Hosting
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    using Simple.Web.CodeGeneration;

    /// <summary>
    /// Builds handlers. To be used by Hosting plug-ins.
    /// </summary>
    internal sealed class HandlerFactory
    {
        public static readonly MethodInfo GetHandlerMethod = typeof(HandlerFactory).GetMethod("GetHandler");
        private static HandlerFactory _instance;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<IDictionary<string, string>, IScopedHandler>>>
            _builders = new ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<IDictionary<string, string>, IScopedHandler>>>();

        private readonly HandlerBuilderFactory _handlerBuilderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerFactory"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <remarks>For testing only. In production, use the singleton <see cref="Instance"/>.</remarks>
        public HandlerFactory(IConfiguration configuration)
        {
            _handlerBuilderFactory = new HandlerBuilderFactory(configuration);
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static HandlerFactory Instance
        {
            get { return _instance ?? (_instance = new HandlerFactory(SimpleWeb.Configuration)); }
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="handlerInfo">The handler info.</param>
        /// <returns></returns>
        public IScopedHandler GetHandler(HandlerInfo handlerInfo)
        {
            var builderDictionary = _builders.GetOrAdd(handlerInfo.HttpMethod,
                                                       _ =>
                                                       new ConcurrentDictionary<Type, Func<IDictionary<string, string>, IScopedHandler>>());

            var builder = builderDictionary.GetOrAdd(handlerInfo.HandlerType, _handlerBuilderFactory.BuildHandlerBuilder);
            var handler = builder(handlerInfo.Variables);
            return handler;
        }
    }
}