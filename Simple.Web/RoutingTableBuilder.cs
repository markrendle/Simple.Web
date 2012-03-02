namespace Simple.Web
{
    using System;
    using System.Linq;

    internal class RoutingTableBuilder
    {
        private readonly Type _endpointType;

        public RoutingTableBuilder(Type endpointType)
        {
            _endpointType = endpointType;
        }

        public RoutingTable BuildRoutingTable()
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithPostEndpoints(routingTable);
            return routingTable;
        }

        private void PopulateRoutingTableWithPostEndpoints(RoutingTable routingTable)
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

        private bool TypeIsPostEndpoint(Type type)
        {
            if (type.IsAbstract || !typeof(IEndpoint).IsAssignableFrom(type)) return false;

            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == _endpointType)
                    return true;
                baseType = baseType.BaseType;
            }

            return false;
        }     
    }
}