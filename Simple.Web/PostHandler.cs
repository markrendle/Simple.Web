namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    class PostHandler
    {
        private static readonly object RoutingTableLock = new object();
        private static volatile RoutingTable _postRoutingTable;

        internal static void HandleRequest(HttpContext context)
        {
            Type endpointType;
            var variables = GetPostEndpointType(context, out endpointType);
            var endpoint = EndpointFactory.Instance.PostEndpoint(endpointType, variables);
            if (endpoint != null)
            {
                var output = endpoint.Run().ToString();
                context.Response.Write(output);
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
                        var routingTable = new RoutingTable();
                        PopulateRoutingTableWithPostEndpoints(routingTable);

                        _postRoutingTable = routingTable;
                    }
                }
            }
        }

        
        
        private static void PopulateRoutingTableWithPostEndpoints(RoutingTable routingTable)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
            {
                foreach (
                    var exportedType in
                        assembly.GetExportedTypes().Where(
                            type => typeof (PostEndpoint).IsAssignableFrom(type) && !type.IsAbstract))
                {
                    var instance = Activator.CreateInstance(exportedType) as PostEndpoint;
                    if (instance != null)
                    {
                        routingTable.Add(instance.UriTemplate, exportedType);
                    }
                }
            }
        }
    }
}