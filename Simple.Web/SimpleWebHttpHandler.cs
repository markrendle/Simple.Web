namespace Simple.Web
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Placeholder. Coolness will go here.
    /// </summary>
    public class SimpleWebHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    GetHandler.HandleRequest(context);
                    break;
                case "POST":
                    PostHandler.HandleRequest(context);
                    break;
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
