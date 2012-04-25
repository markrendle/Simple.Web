namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Web;
    using CodeGeneration;

    sealed class EndpointFactory
    {
        private static EndpointFactory _instance;
        public static EndpointFactory Instance
        {
            get { return _instance ?? (_instance = new EndpointFactory(SimpleWeb.Configuration)); }
        }

        private readonly EndpointBuilderFactory _endpointBuilderFactory;
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>> _getBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>>();
        private readonly ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>> _postBuilders =
            new ConcurrentDictionary<Type, Func<IDictionary<string, string>, object>>();

        public EndpointFactory(IConfiguration configuration)
        {
            _endpointBuilderFactory = new EndpointBuilderFactory(configuration);
        }

        public object GetEndpoint(Type type, IDictionary<string,string> variables)
        {
            var builder = _getBuilders.GetOrAdd(type, _endpointBuilderFactory.BuildEndpointBuilder);
            return builder(variables);
        }

        public object GetEndpoint(EndpointInfo endpointInfo)
        {
            Func<IDictionary<string, string>, object> builder;

            if (endpointInfo.HttpMethod == "GET")
            {
                builder = _getBuilders.GetOrAdd(endpointInfo.EndpointType, _endpointBuilderFactory.BuildEndpointBuilder);
            }
            else if (endpointInfo.HttpMethod == "POST")
            {
                builder = _postBuilders.GetOrAdd(endpointInfo.EndpointType, _endpointBuilderFactory.BuildEndpointBuilder);
            }
            else
            {
                throw new HttpException(405, "Method not allowed.");
            }
            return builder(endpointInfo.Variables);
        }
    }
}