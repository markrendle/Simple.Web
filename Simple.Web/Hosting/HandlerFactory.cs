namespace Simple.Web.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Web;
    using Simple.Web.CodeGeneration;

    /// <summary>
    /// Builds handlers. To be used by Hosting plug-ins.
    /// </summary>
    public sealed class HandlerFactory
    {
        private static HandlerFactory _instance;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static HandlerFactory Instance
        {
            get { return _instance ?? (_instance = new HandlerFactory(SimpleWeb.Configuration)); }
        }

        private readonly HandlerBuilderFactory _handlerBuilderFactory;
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>> _getBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>>();
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>> _postBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>>();

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
        /// Gets the handler for a particular type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="variables">The variables.</param>
        /// <returns></returns>
        public object GetHandler(Type type, IDictionary<string,string> variables)
        {
            var builder = _getBuilders.GetOrAdd(type, _handlerBuilderFactory.BuildHandlerBuilder);
            return builder(variables);
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="handlerInfo">The handler info.</param>
        /// <returns></returns>
        public object GetHandler(HandlerInfo handlerInfo)
        {
            Func<IDictionary<string, string>, object> builder;

            if (handlerInfo.HttpMethod == "GET")
            {
                builder = _getBuilders.GetOrAdd(handlerInfo.HandlerType, _handlerBuilderFactory.BuildHandlerBuilder);
            }
            else if (handlerInfo.HttpMethod == "POST")
            {
                builder = _postBuilders.GetOrAdd(handlerInfo.HandlerType, _handlerBuilderFactory.BuildHandlerBuilder);
            }
            else
            {
                throw new HttpException(405, "Method not allowed.");
            }
            return builder(handlerInfo.Variables);
        }
    }
}