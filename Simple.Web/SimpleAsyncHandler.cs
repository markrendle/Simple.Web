namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    internal class SimpleAsyncHandler<TEndpointType> : IHttpAsyncHandler
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly IAuthenticationProvider _authenticationProvider;
// ReSharper disable StaticFieldInGenericType
        private static readonly Lazy<RoutingTable> RoutingTable = new Lazy<RoutingTable>(() => new RoutingTableBuilder(typeof(TEndpointType)).BuildRoutingTable());
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

            SimpleAsyncHandler<TEndpointType> instance;
            if (endpointInfo.RequiresAuthentication)
            {
                var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new AuthenticationProvider();
                instance = new SimpleAsyncHandler<TEndpointType>(context, endpointInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleAsyncHandler<TEndpointType>(context, endpointInfo);
            }

            return instance;
        }

        private SimpleAsyncHandler(IContext context, EndpointInfo endpointInfo) : this(context, endpointInfo, null)
        {
        }

        private SimpleAsyncHandler(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _authenticationProvider = authenticationProvider;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var result = new SimpleAsyncHandlerResult<TEndpointType>(_context, _endpointInfo, _authenticationProvider, cb, extraData);
            result.Run();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }
    }
}