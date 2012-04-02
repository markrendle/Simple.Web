namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;

    internal class SimpleHandler<TEndpointType> : IHttpHandler
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
// ReSharper disable StaticFieldInGenericType
        private static readonly Lazy<RoutingTable> RoutingTable = new Lazy<RoutingTable>(() => new RoutingTableBuilder(typeof(TEndpointType)).BuildRoutingTable());
// ReSharper restore StaticFieldInGenericType

        public static IHttpHandler TryCreate(IContext context)
        {
            IDictionary<string, string> variables;
            var endpointType = RoutingTable.Value.Get(context.Request.Url.AbsolutePath, context.Request.AcceptTypes, out variables);
            if (endpointType == null) return null;
            var endpointInfo = new EndpointInfo(endpointType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                endpointInfo.Variables.Add(key, context.Request.QueryString[key]);
            }

            return new SimpleHandler<TEndpointType>(context, endpointInfo);
        }

        private SimpleHandler(IContext context, EndpointInfo endpointInfo)
        {
            _context = context;
            _endpointInfo = endpointInfo;
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Run();
            }
            catch (HttpException httpException)
            {
                context.Response.StatusCode = httpException.ErrorCode;
                context.Response.StatusDescription = httpException.Message;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Internal server error.";
            }
        }

        private void Run()
        {
            var endpoint = EndpointFactory.Instance.GetEndpoint(_endpointInfo);

            if (endpoint != null)
            {
                SetContext(endpoint);
                var runner = EndpointRunner.Create<TEndpointType>(endpoint);
                runner.BeforeRun(_context, this._contentTypeHandlerTable);
                RunEndpoint(runner);
            }
        }

        private void SetContext(object endpoint)
        {
            var needContext = endpoint as INeedContext;
            if (needContext != null) needContext.Context = _context;
        }

        private void RunEndpoint(EndpointRunner endpoint)
        {
            var status = endpoint.Run();

            WriteStatusCode(status);

            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                var redirect = endpoint.Endpoint as IMayRedirect;
                if (redirect != null && !string.IsNullOrWhiteSpace(redirect.Location))
                {
                    _context.Response.Headers.Set("Location", redirect.Location);
                }
            }
            if (status.Code != 200)
            {
                return;
            }

            WriteResponse(endpoint);
        }

        private void WriteStatusCode(Status status)
        {
            _context.Response.StatusCode = status.Code;
            _context.Response.StatusDescription = status.Description;
        }

        private void WriteResponse(EndpointRunner endpoint)
        {
            if (endpoint.HasOutput && endpoint.Output is RawHtml)
            {
                _context.Response.ContentType =
                    _context.Request.AcceptTypes.FirstOrDefault(
                        at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
                _context.Response.Output.Write(endpoint.Output.ToString());
            }
            else
            {
                IContentTypeHandler contentTypeHandler;
                if (!TryGetContentTypeHandler(out contentTypeHandler))
                {
                    throw new UnsupportedMediaTypeException(_context.Request.AcceptTypes);
                }
                _context.Response.ContentType = contentTypeHandler.GetContentType(_context.Request.AcceptTypes);
                contentTypeHandler.Write(new Content(endpoint), _context.Response.Output);
            }
        }

        private bool TryGetContentTypeHandler(out IContentTypeHandler contentTypeHandler)
        {
            try
            {
                contentTypeHandler = _contentTypeHandlerTable.GetContentTypeHandler(_context.Request.AcceptTypes);
            }
            catch (UnsupportedMediaTypeException)
            {
                _context.Response.StatusCode = 415;
                _context.Response.Close();
                contentTypeHandler = null;
                return false;
            }
            return true;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}