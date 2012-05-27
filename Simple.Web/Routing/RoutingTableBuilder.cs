namespace Simple.Web.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class RoutingTableBuilder
    {
        private readonly IList<Type> _handlerBaseTypes;

        public RoutingTableBuilder(params Type[] handlerBaseTypes)
        {
            _handlerBaseTypes = handlerBaseTypes;
        }

        public RoutingTable BuildRoutingTable()
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithHandlers(routingTable);
            return routingTable;
        }

        private void PopulateRoutingTableWithHandlers(RoutingTable routingTable)
        {
            PopulateRoutingTableWithHandlers(routingTable, ExportedTypeHelper.FromCurrentAppDomain(TypeIsHandler));
        }

        private void PopulateRoutingTableWithHandlers(RoutingTable routingTable, IEnumerable<Type> handlerTypes)
        {
            foreach (var exportedType in handlerTypes)
            {
                var respondsWithTypes = RespondsWithAttribute.Get(exportedType).SelectMany(rta => rta.AcceptTypes).ToList();
                var respondsToTypes = RespondsToAttribute.Get(exportedType).SelectMany(rta => rta.AcceptTypes).ToList();
                foreach (var uriTemplate in UriTemplateAttribute.GetAllTemplates(exportedType))
                {
                    routingTable.Add(uriTemplate, new HandlerTypeInfo(exportedType, respondsToTypes, respondsWithTypes));
                }

                // If it's the LoginPage, set it to the configuration
                if (SimpleWeb.Configuration.LoginPage == null && typeof(ILoginPage).IsAssignableFrom(exportedType))
                {
                    SimpleWeb.Configuration.LoginPage = exportedType;
                }
            }
        }

        private bool TypeIsHandler(Type type)
        {
            if (type.IsAbstract || type.IsInterface) return false;

            return _handlerBaseTypes.Any(t => t.IsAssignableFrom(type));
        }

        internal RoutingTable BuildRoutingTable(IEnumerable<Type> handlerTypes)
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithHandlers(routingTable, handlerTypes);
            return routingTable;
        }
    }
}