namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;

    internal abstract class HandlerBase<TMethod>
            where TMethod : class
    {
        private readonly Lazy<RoutingTable> _lazyRoutingTable;

        private RoutingTable RoutingTable
        {
            get { return _lazyRoutingTable.Value; }
        }

        protected readonly ContentTypeHandlerTable ContentTypeHandlerTable = new ContentTypeHandlerTable();

        protected HandlerBase()
        {
            _lazyRoutingTable = new Lazy<RoutingTable>(() => new RoutingTableBuilder(typeof(TMethod)).BuildRoutingTable(),
                                                       LazyThreadSafetyMode.PublicationOnly);
        }

        protected HandlerBase(IEnumerable<Type> endpointTypes)
        {
            _lazyRoutingTable =
                new Lazy<RoutingTable>(() => new RoutingTableBuilder(typeof(TMethod)).BuildRoutingTable(endpointTypes),
                                       LazyThreadSafetyMode.PublicationOnly);
        }

        internal void HandleRequest(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.Request == null) throw new InvalidOperationException("Request cannot be null");
            if (context.Response == null) throw new InvalidOperationException("Response cannot be null");

            EndpointInfo endpointInfo;
            if (!GetEndpointInfo(RoutingTable, context, out endpointInfo))
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (endpointInfo != null)
            {
                var endpoint = EndpointFactory.Instance.GetEndpoint(endpointInfo);
                var runner = EndpointRunner.Create<TMethod>(endpoint);

                SetContext(endpoint, context);
                if (endpoint != null)
                {
                    OnRunning(runner, context);
                    RunEndpoint(runner, context);
                }
            }
        }

        private void SetContext(object endpoint, IContext context)
        {
            var needContext = endpoint as INeedContext;
            if (needContext != null) needContext.Context = context;
        }

        protected virtual void OnRunning(EndpointRunner endpoint, IContext context)
        {
            
        }

        private bool TryGetContentTypeHandler(IContext context, out IContentTypeHandler contentTypeHandler)
        {
            try
            {
                contentTypeHandler = ContentTypeHandlerTable.GetContentTypeHandler(context.Request.AcceptTypes);
            }
            catch (UnsupportedMediaTypeException)
            {
                context.Response.StatusCode = 415;
                context.Response.Close();
                contentTypeHandler = null;
                return false;
            }
            return true;
        }

        private void WriteResponse(IContext context, EndpointRunner endpoint)
        {
            if (endpoint.HasOutput && endpoint.Output is RawHtml)
            {
                context.Response.ContentType =
                    context.Request.AcceptTypes.FirstOrDefault(
                        at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
                context.Response.Output.Write(endpoint.Output.ToString());
            }
            else
            {
                IContentTypeHandler contentTypeHandler;
                if (!TryGetContentTypeHandler(context, out contentTypeHandler))
                {
                    throw new UnsupportedMediaTypeException(context.Request.AcceptTypes);
                }
                context.Response.ContentType = contentTypeHandler.GetContentType(context.Request.AcceptTypes);
                contentTypeHandler.Write(new Content(endpoint, null), context.Response.Output);
            }
        }

        private bool GetEndpointInfo(RoutingTable routingTable, IContext context, out EndpointInfo endpointInfo)
        {
            IDictionary<string, string> variables;
            var endpointType = routingTable.Get(context.Request.Url.AbsolutePath, context.Request.AcceptTypes, out variables);

            if (endpointType == null)
            {
                endpointInfo = null;
                return false;
            }
            endpointInfo = new EndpointInfo(endpointType, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                endpointInfo.Variables.Add(key, context.Request.QueryString[key]);
            }
            return true;
        }

        private void RunEndpoint(EndpointRunner endpoint, IContext context)
        {
            var status = endpoint.Run();

            WriteStatusCode(context, status);

            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                var redirect = endpoint.Endpoint as IMayRedirect;
                if (redirect != null && !string.IsNullOrWhiteSpace(redirect.Location))
                {
                    context.Response.Headers.Set("Location", redirect.Location);
                }
            }
            if (status.Code != 200)
            {
                return;
            }

            WriteResponse(context, endpoint);
        }

        private void WriteStatusCode(IContext context, Status status)
        {
            context.Response.StatusCode = status.Code;
            context.Response.StatusDescription = status.Description;
        }
    }
}