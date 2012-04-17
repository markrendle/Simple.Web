namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    internal class VerbHandlerFactory<TSync,TAsync>
    {
// ReSharper disable StaticFieldInGenericType
        private static readonly Lazy<RoutingTable> RoutingTable = new Lazy<RoutingTable>(() => new RoutingTableBuilder(typeof(TSync), typeof(TAsync)).BuildRoutingTable());
// ReSharper restore StaticFieldInGenericType

        public static IHttpHandler TryCreate(IContext context)
        {
            IDictionary<string, string> variables;
            var endpointType = RoutingTable.Value.Get(context.Request.Url.AbsolutePath, context.Request.AcceptTypes, out variables);
            if (endpointType == null) return null;
            var endpointInfo = new EndpointInfo(endpointType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                endpointInfo.Variables.Add(key, context.Request.QueryString[key]);
            }

            return CreateHandler(context, endpointInfo);
        }

        private static IHttpHandler CreateHandler(IContext context, EndpointInfo endpointInfo)
        {
            IHttpHandler instance;
            if (typeof (TSync).IsAssignableFrom(endpointInfo.EndpointType))
            {
                instance = CreateHttpHandler(context, endpointInfo);
            }
            else if (typeof (TAsync).IsAssignableFrom(endpointInfo.EndpointType))
            {
                instance = CreateAsyncHttpHandler(context, endpointInfo);
            }
            else
            {
                throw new RoutingException(context.Request.Url.ToString(), "No matching route found.");
            }
            return instance;
        }

        private static IHttpHandler CreateHttpHandler(IContext context, EndpointInfo endpointInfo)
        {
            IHttpHandler instance;
            if (endpointInfo.RequiresAuthentication)
            {
                var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new AuthenticationProvider();
                instance = new SimpleHandler<TSync>(context, endpointInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleHandler<TSync>(context, endpointInfo);
            }
            return instance;
        }

        private static IHttpHandler CreateAsyncHttpHandler(IContext context, EndpointInfo endpointInfo)
        {
            IHttpHandler instance;
            if (endpointInfo.RequiresAuthentication)
            {
                var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new AuthenticationProvider();
                instance = new SimpleAsyncHandler<TAsync>(context, endpointInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleAsyncHandler<TAsync>(context, endpointInfo);
            }
            return instance;
        }
    }
}