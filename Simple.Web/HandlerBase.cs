namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    internal abstract class HandlerBase
    {
        private readonly Lazy<RoutingTable> _lazyRoutingTable;

        private RoutingTable RoutingTable
        {
            get { return _lazyRoutingTable.Value; }
        }

        protected readonly ContentTypeHandlerTable ContentTypeHandlerTable = new ContentTypeHandlerTable();

        protected HandlerBase(Type endpointType)
        {
            _lazyRoutingTable = new Lazy<RoutingTable>(() => new RoutingTableBuilder(endpointType).BuildRoutingTable(),
                                                       LazyThreadSafetyMode.PublicationOnly);
        }

        protected HandlerBase(Type baseEndpointType, IEnumerable<Type> endpointTypes)
        {
            _lazyRoutingTable =
                new Lazy<RoutingTable>(() => new RoutingTableBuilder(baseEndpointType).BuildRoutingTable(endpointTypes),
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
                if (endpoint != null)
                {
                    OnRunning(endpoint, context);
                    RunEndpoint(endpoint, context);
                }
            }
        }

        protected virtual void OnRunning(IEndpoint endpoint, IContext context)
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

        private void WriteResponse(IContext context, IEndpoint endpoint)
        {
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                if (endpoint.Output is RawHtml)
                {
                    context.Response.ContentType =
                        context.Request.AcceptTypes.FirstOrDefault(
                            at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
                    writer.Write(endpoint.Output.ToString());
                }
                else
                {
                    IContentTypeHandler contentTypeHandler;
                    if (!TryGetContentTypeHandler(context, out contentTypeHandler))
                    {
                        throw new UnsupportedMediaTypeException(context.Request.AcceptTypes);
                    }
                    context.Response.ContentType = contentTypeHandler.GetContentType(context.Request.AcceptTypes);
                    contentTypeHandler.Write(endpoint, writer);
                }
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

        private void RunEndpoint(IEndpoint endpoint, IContext context)
        {
            var status = endpoint.Run();

            WriteStatusCode(context, status);

            if (status.Code != 200)
            {
                context.Response.Close();
                return;
            }

            WriteResponse(context, endpoint);

            context.Response.Flush();
            context.Response.Close();
        }

        private void WriteStatusCode(IContext context, Status status)
        {
            context.Response.StatusCode = status.Code;
            context.Response.StatusDescription = status.Description;
        }
    }
}