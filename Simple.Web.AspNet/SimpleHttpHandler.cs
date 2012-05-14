namespace Simple.Web.AspNet
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using Helpers;

    internal class SimpleHttpHandler : IHttpHandler
    {
        private readonly IContext _context;
        private readonly HandlerInfo _handlerInfo;
        private readonly IAuthenticationProvider _authenticationProvider;

        internal SimpleHttpHandler(IContext context, HandlerInfo handlerInfo) : this(context, handlerInfo, null)
        {
        }

        internal SimpleHttpHandler(IContext context, HandlerInfo handlerInfo, IAuthenticationProvider authenticationProvider)
        {
            _context = context;
            _handlerInfo = handlerInfo;
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
                new ErrorHelper(_context).WriteError(ex);
            }
        }

        private void Run()
        {
            var handler = HandlerFactory.Instance.GetHandler(_handlerInfo);

            if (handler != null)
            {
                var run = HandlerRunnerFactory.Instance.Get(handler.GetType());
                run(handler, _context);
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