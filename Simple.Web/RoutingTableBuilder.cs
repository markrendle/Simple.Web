namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class RoutingTableBuilder
    {
        private readonly IList<Type> _endpointBaseTypes;

        public RoutingTableBuilder(params Type[] endpointBaseTypes)
        {
            _endpointBaseTypes = endpointBaseTypes;
        }

        public RoutingTable BuildRoutingTable()
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithEndpoints(routingTable);
            return routingTable;
        }

        private void PopulateRoutingTableWithEndpoints(RoutingTable routingTable)
        {
            PopulateRoutingTableWithEndpoints(routingTable, ExportedTypeHelper.FromCurrentAppDomain(TypeIsEndpoint));
        }

        private void PopulateRoutingTableWithEndpoints(RoutingTable routingTable, IEnumerable<Type> endpointTypes)
        {
            foreach (var exportedType in endpointTypes)
            {
                var respondsToTypes = RespondsToAttribute.Get(exportedType).SelectMany(rta => rta.AcceptTypes).ToList();
                foreach (var uriTemplate in UriTemplateAttribute.GetAllTemplates(exportedType))
                {
                    routingTable.Add(uriTemplate, new EndpointTypeInfo(exportedType, respondsToTypes));
                }
            }
        }

        private bool TypeIsEndpoint(Type type)
        {
            if (type.IsAbstract || type.IsInterface) return false;

            return _endpointBaseTypes.Any(t => t.IsAssignableFrom(type));
        }

        public RoutingTable BuildRoutingTable(IEnumerable<Type> endpointTypes)
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithEndpoints(routingTable, endpointTypes);
            return routingTable;
        }
    }
}