using System;
using System.Text;

namespace Simple.Web
{
    using System.Diagnostics;
    using System.Web;

    public class SimpleHandlerFactory : IHttpHandlerFactory
    {
        private static readonly object StartupSync = new object();
        private static volatile StartupTaskRunner _startupTaskRunner = new StartupTaskRunner();

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            Startup();
            var simpleContext = new ContextWrapper(context);
            switch (context.Request.HttpMethod.ToUpperInvariant())
            {
                case "GET":
                    if (PublicFileHandler.IsPublicFile(context.Request.Url.AbsolutePath, SimpleWeb.Configuration))
                    {
                        return new PublicFileHandler();
                    }
                    return VerbHandlerFactory<IGet, IGetAsync>.TryCreate(simpleContext);
                case "POST":
                    return VerbHandlerFactory<IPost, IPostAsync>.TryCreate(simpleContext);
            }

            return null;
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
