namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    class GetHandler
    {
        private static readonly object RoutingTableLock = new object();
        private static volatile bool _routingTableInitialized;
        private static RoutingTable _getRoutingTable;

        internal static void HandleRequest(HttpContext context)
        {
            Type endpointType;
            var variables = GetEndpointType(context, out endpointType);
            if (endpointType != null)
            {
                var endpoint = EndpointFactory.Instance.GetEndpoint(endpointType, variables);
                if (endpoint != null)
                {
                    var output = endpoint.Run().ToString();
                    context.Response.Write(output);
                    context.Response.Flush();
                    context.Response.Close();
                }
            }
        }

        private static IDictionary<string, string> GetEndpointType(HttpContext context, out Type endpointType)
        {
            BuildGetRoutingTable();

            IDictionary<string, string> variables;
            endpointType = _getRoutingTable.Get(context.Request.Url.AbsolutePath, out variables);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                variables.Add(key, context.Request.QueryString[key]);
            }
            return variables;
        }

        private static void BuildGetRoutingTable()
        {
            if (!_routingTableInitialized)
            {
                lock (RoutingTableLock)
                {
                    if (!_routingTableInitialized)
                    {
                        _getRoutingTable = new RoutingTableBuilder(typeof(GetEndpoint<>)).BuildRoutingTable();
                        _routingTableInitialized = true;
                    }
                }
            }
        }
    }
}