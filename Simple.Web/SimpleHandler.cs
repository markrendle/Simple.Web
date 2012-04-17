namespace Simple.Web
{
    using System;
    using System.Diagnostics;
    using System.Web;

    internal class SimpleHandler<TEndpointType> : IHttpHandler, IDisposable
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
        private readonly HandlerHelper _helper;
        private object _endpoint;

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
            _endpoint = EndpointFactory.Instance.GetEndpoint(_endpointInfo);

            if (_endpoint != null)
            {
                if (!_helper.CheckAuthentication(_endpoint)) return;

                _helper.SetContext(_endpoint);
                _helper.SetFiles(_endpoint);
                var runner = EndpointRunner.Create<TEndpointType>(_endpoint);
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

        public void Dispose()
        {
            DisposeHelper.TryDispose(_endpoint);
        }
    }

    internal static class DisposeHelper
    {
        public static void TryDispose(object obj)
        {
            if (obj == null) return;
            var disposable = obj as IDisposable;
            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    throw;
                }
            }
        }
    }
}