namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    class GetHandler
    {
        private static readonly object RoutingTableLock = new object();
        private static volatile RoutingTable _getRoutingTable;

        internal static void HandleRequest(HttpContext context)
        {
            Type endpointType;
            var variables = GetGetEndpointType(context, out endpointType);
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

        private static IDictionary<string, string> GetGetEndpointType(HttpContext context, out Type endpointType)
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
            if (_getRoutingTable == null)
            {
                lock (RoutingTableLock)
                {
                    if (_getRoutingTable == null)
                    {
                        var routingTable = new RoutingTable();
                        PopulateRoutingTableWithGetEndpoints(routingTable);

                        _getRoutingTable = routingTable;
                    }
                }
            }
        }

        private static void PopulateRoutingTableWithGetEndpoints(RoutingTable routingTable)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (
                    var exportedType in
                        assembly.GetExportedTypes().Where(
                            type => typeof(GetEndpoint).IsAssignableFrom(type) && !type.IsAbstract))
                {
                    var instance = Activator.CreateInstance(exportedType) as GetEndpoint;
                    if (instance != null)
                    {
                        routingTable.Add(instance.UriTemplate, exportedType);
                    }
                }
            }
        }
    }
}