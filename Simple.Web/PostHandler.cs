namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;

    class PostHandler
    {
        private static readonly object RoutingTableLock = new object();
        private static volatile RoutingTable _postRoutingTable;

        internal static void HandleRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.Request == null) throw new InvalidOperationException("Request cannot be null");

            Type endpointType;
            var variables = GetPostEndpointType(context, out endpointType);
            var endpoint = EndpointFactory.Instance.PostEndpoint(endpointType, variables, context.Request);
            if (endpoint != null)
            {
                object output;
                try
                {
                    output = endpoint.Run();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    context.Response.StatusCode = 500;
                    context.Response.Status = "Internal server error";
                    context.Response.Close();
                    return;
                }

                var outputWriter = OutputStreamWriter.GetWriter(context.Request.AcceptTypes, endpoint);
                context.Response.ContentType = outputWriter.ContentType;
                outputWriter.Write(context.Response.OutputStream, output);

                context.Response.Flush();
                context.Response.Close();
            }
        }

        private static IDictionary<string, string> GetPostEndpointType(HttpContext context, out Type endpointType)
        {
            BuildPostRoutingTable();

            IDictionary<string, string> variables;
            endpointType = _postRoutingTable.Get(context.Request.Url.AbsolutePath, out variables);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                variables.Add(key, context.Request.QueryString[key]);
            }
            return variables;
        }

        private static void BuildPostRoutingTable()
        {
            if (_postRoutingTable == null)
            {
                lock (RoutingTableLock)
                {
                    if (_postRoutingTable == null)
                    {
                        _postRoutingTable = new RoutingTableBuilder(typeof(PostEndpoint<,>)).BuildRoutingTable();
                    }
                }
            }
        }
    }
}