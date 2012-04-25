namespace Simple.Web
{
    using System;
    using System.Diagnostics;
    using System.Web;

    internal class SimpleHandler : IHttpHandler
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly IAuthenticationProvider _authenticationProvider;

        internal SimpleHandler(IContext context, EndpointInfo endpointInfo) : this(context, endpointInfo, null)
        {
        }

        internal SimpleHandler(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _authenticationProvider = authenticationProvider;
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                new HandlerHelper(_context, _authenticationProvider).WriteError(ex);
            }
        }

        private void Run()
        {
            var endpoint = EndpointFactory.Instance.GetEndpoint(_endpointInfo);

            if (endpoint != null)
            {
                var run = EndpointRunnerFactory.Instance.Get(endpoint.GetType());
                run(endpoint, _context);
            }
        }

        public bool IsReusable
        {
            get { return false; }
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