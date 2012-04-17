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

        internal SimpleAsyncHandler(IContext context, EndpointInfo endpointInfo) : this(context, endpointInfo, null)
        {
        }

        internal SimpleAsyncHandler(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider)
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