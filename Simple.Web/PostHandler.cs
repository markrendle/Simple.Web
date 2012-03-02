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
            var endpoint = EndpointFactory.Instance.PostEndpoint(endpointType, variables, context.Request);
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
                var postEndpointTypes = assembly.GetExportedTypes().Where(TypeIsPostEndpoint).ToList();

                foreach (var exportedType in postEndpointTypes)
                {
                    var instance = Activator.CreateInstance(exportedType) as IEndpoint;
                    if (instance != null)
                    {
                        routingTable.Add(instance.UriTemplate, exportedType);
                    }
                }
            }
        }

        private static bool TypeIsPostEndpoint(Type type)
        {
            if (type.IsAbstract || !typeof(IEndpoint).IsAssignableFrom(type)) return false;

            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(PostEndpoint<,>))
                    return true;
                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}