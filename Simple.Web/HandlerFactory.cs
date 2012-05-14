namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Web;
    using CodeGeneration;

    public sealed class HandlerFactory
    {
        private static HandlerFactory _instance;
        public static HandlerFactory Instance
        {
            get { return _instance ?? (_instance = new HandlerFactory(SimpleWeb.Configuration)); }
        }

        private readonly HandlerBuilderFactory _handlerBuilderFactory;
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>> _getBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>>();
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>> _postBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>>();

        public HandlerFactory(IConfiguration configuration)
        {
            _handlerBuilderFactory = new HandlerBuilderFactory(configuration);
        }

        public object GetHandler(Type type, IDictionary<string,string> variables)
        {
            var builder = _getBuilders.GetOrAdd(type, _handlerBuilderFactory.BuildHandlerBuilder);
            return builder(variables);
        }

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