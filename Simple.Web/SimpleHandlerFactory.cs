using System;
using System.Text;

namespace Simple.Web
{
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
                    return SimpleHandler<IGet>.TryCreate(simpleContext) ??
                           SimpleAsyncHandler<IGetAsync>.TryCreate(simpleContext);
                case "POST":
                    return SimpleHandler<IPost>.TryCreate(simpleContext) ??
                           SimpleAsyncHandler<IPostAsync>.TryCreate(simpleContext);
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
        }
    }
}
