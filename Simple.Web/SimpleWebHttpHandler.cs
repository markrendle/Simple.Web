namespace Simple.Web
{
    using System;
    using System.Web;

    /// <summary>
    /// Placeholder. Coolness will go here.
    /// </summary>
    public sealed class SimpleWebHttpHandler : IHttpHandler
    {
        private static readonly object StartupSync = new object();
        private static volatile StartupTaskRunner _startupTaskRunner = new StartupTaskRunner();
        private static readonly GetHandler GetHandler = new GetHandler();
        private static readonly PostHandler PostHandler = new PostHandler();
        private static readonly PublicFileHandler PublicFileHandler = new PublicFileHandler();

        public void ProcessRequest(HttpContext context)
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

            ProcessRequest(new ContextWrapper(context));
            context.ApplicationInstance.CompleteRequest();
        }

        internal void ProcessRequest(IContext context)
        {
            try
            {
                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        if (PublicFileHandler.TryHandleAsFile(context.Request, context.Response))
                        {
                            break;
                        }
                        GetHandler.HandleRequest(context);
                        break;
                    case "POST":
                        PostHandler.HandleRequest(context);
                        break;
                }
            }
            catch (HttpException httpException)
            {
                context.Response.StatusCode = httpException.ErrorCode;
                context.Response.StatusDescription = httpException.Message;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Internal server error.";
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
