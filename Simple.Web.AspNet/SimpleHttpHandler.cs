namespace Simple.Web.AspNet
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using Helpers;
    using Http;

    internal class SimpleHttpHandler : IHttpHandler
    {
        private readonly IContext _context;
        private readonly HandlerInfo _handlerInfo;

        internal SimpleHttpHandler(IContext context, HandlerInfo handlerInfo)
        {
            _context = context;
            _handlerInfo = handlerInfo;
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
                var run = HandlerRunnerFactory.Instance.Get(handler.GetType(), _context.Request.HttpMethod);
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