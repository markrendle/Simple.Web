namespace Simple.Web
{
    using System;
    using System.Diagnostics;
    using System.Web;

    internal class SimpleHandler<TEndpointType> : IHttpHandler
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
        private readonly HandlerHelper _helper;

        internal SimpleHandler(IContext context, EndpointInfo endpointInfo) : this(context, endpointInfo, null)
        {
        }

        internal SimpleHandler(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _helper = new HandlerHelper(endpointInfo, context, authenticationProvider);
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                _helper.WriteError(ex);
            }
        }

        private void Run()
        {
            var endpoint = EndpointFactory.Instance.GetEndpoint(_endpointInfo);

            if (endpoint != null)
            {
                if (!_helper.CheckAuthentication(endpoint)) return;

                _helper.SetContext(endpoint);
                _helper.SetFiles(endpoint);
                var runner = EndpointRunner.Create<TEndpointType>(endpoint);
                runner.BeforeRun(_context, _contentTypeHandlerTable);
                RunEndpoint(runner);
            }
        }

        private void RunEndpoint(EndpointRunner runner)
        {
            var status = runner.Run();

            _helper.WriteResponse(runner, status);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}