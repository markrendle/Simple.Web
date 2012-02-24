namespace Simple.Web
{
    using System;
    using System.Web;

    /// <summary>
    /// Placeholder. Coolness will go here.
    /// </summary>
    public class SimpleWebHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var endpoint = EndpointFactory.Instance.GetEndpoint(context.Request.Url.AbsolutePath);
            if (endpoint != null)
            {
                var output = endpoint.Run().ToString();
                context.Response.Write(output);
                context.Response.Flush();
                context.Response.Close();
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
