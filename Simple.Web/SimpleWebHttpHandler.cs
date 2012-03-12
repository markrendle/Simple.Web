namespace Simple.Web
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Placeholder. Coolness will go here.
    /// </summary>
    public class SimpleWebHttpHandler : IHttpHandler
    {
        private static readonly GetHandler GetHandler = new GetHandler();
        private static readonly PostHandler PostHandler = new PostHandler();

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new ContextWrapper(context));
        }

        internal void ProcessRequest(IContext context)
        {
            try
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
            catch (HttpException httpException)
            {
                context.Response.StatusCode = httpException.ErrorCode;
                context.Response.StatusDescription = httpException.Message;
                context.Response.Close();
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Internal server error.";
                context.Response.Close();
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }

    class ContextWrapper : IContext
    {
        private readonly HttpContext _context;
        private readonly RequestWrapper _request;
        private readonly ResponseWrapper _response;

        public ContextWrapper(HttpContext context)
        {
            _context = context;
            _request = new RequestWrapper(context.Request);
            _response = new ResponseWrapper(context.Response);
        }

        public IRequest Request
        {
            get { return _request; }
        }

        public IResponse Response
        {
            get { return _response; }
        }
    }

    internal class ResponseWrapper : IResponse
    {
        private readonly HttpResponse _response;

        public ResponseWrapper(HttpResponse response)
        {
            _response = response;
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public string StatusDescription
        {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public Stream OutputStream
        {
            get { return _response.OutputStream; }
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public void Close()
        {
            _response.Close();
        }

        public void Flush()
        {
            _response.Flush();
        }
    }
}
