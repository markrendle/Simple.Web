namespace Simple.Web.AspNet
{
    using System.Web;
    using Hosting;

    public class SimpleHttpHandlerFactory : IHttpHandlerFactory
    {
        private static readonly object StartupSync = new object();
        private static StartupTaskRunner _startupTaskRunner = new StartupTaskRunner();

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            Startup();
            if (PublicFileHandler.IsPublicFile(context.Request.Url.AbsolutePath, SimpleWeb.Configuration))
            {
                return new PublicFileHandler();
            }
            var simpleContext = new ContextWrapper(context);
            return HttpMethodHandlerFactory.TryCreate(simpleContext);
        }

        private static void Startup()
        {
            if (_startupTaskRunner != null)
            {
                lock (StartupSync)
                {
                    if (_startupTaskRunner != null)
                    {
                        _startupTaskRunner.RunStartupTasks();
                        _startupTaskRunner = null;
                    }
                }
            }
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
            TryDispose(handler);
        }

        private static void TryDispose(IHttpHandler handler)
        {
            DisposeHelper.TryDispose(handler);
        }
    }
}
