namespace Simple.Web.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Simple.Web.Behaviors;
    using Simple.Web.Helpers;
    using Simple.Web.Hosting;

    /// <summary>
    /// Factory class for building routing tables.
    /// </summary>
    internal sealed class RoutingTableBuilder
    {
        private readonly IList<Type> _handlerBaseTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingTableBuilder"/> class.
        /// </summary>
        /// <param name="handlerBaseTypes">The handler base types.</param>
        public RoutingTableBuilder(params Type[] handlerBaseTypes)
        {
            _handlerBaseTypes = handlerBaseTypes;
        }

        /// <summary>
        /// Builds the routing table.
        /// </summary>
        /// <returns>The routing table for the provided base types.</returns>
        public RoutingTable BuildRoutingTable()
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithHandlers(routingTable);
            return routingTable;
        }

        internal RoutingTable BuildRoutingTable(IEnumerable<Type> handlerTypes)
        {
            var routingTable = new RoutingTable();
            PopulateRoutingTableWithHandlers(routingTable, handlerTypes);
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
                var respondsWithTypes = RespondsWithAttribute.Get(exportedType).SelectMany(rta => rta.ContentTypes).ToList();
                var respondsToTypes = RespondsToAttribute.Get(exportedType).SelectMany(rta => rta.ContentTypes).ToList();
                foreach (var uriTemplate in UriTemplateAttribute.GetAllTemplates(exportedType))
                {
                    if (exportedType.IsGenericTypeDefinition)
                    {
                        BuildRoutesForGenericHandlerType(routingTable, exportedType, uriTemplate, respondsToTypes, respondsWithTypes);
                    }
                    else
                    {
                        routingTable.Add(uriTemplate, new HandlerTypeInfo(exportedType, respondsToTypes, respondsWithTypes));
                    }
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
            if (type.IsAbstract || type.IsInterface)
            {
                return false;
            }

            return _handlerBaseTypes.Any(t => t.IsAssignableFrom(type));
        }

        private static void BuildRoutesForGenericHandlerType(RoutingTable routingTable,
                                                             Type exportedType,
                                                             string uriTemplate,
                                                             List<string> respondsToTypes,
                                                             List<string> respondsWithTypes)
        {
            var genericArgument = exportedType.GetGenericArguments().Single();
            var genericParameterAttributes = genericArgument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
            var constraints = genericArgument.GetGenericParameterConstraints();
            string templatePart = "{" + genericArgument.Name + "}";
            if (uriTemplate.Contains(templatePart))
            {
                var genericResolver =
                    Attribute.GetCustomAttribute(exportedType, typeof(GenericResolverAttribute)) as GenericResolverAttribute;
                IEnumerable<Type> candidateTypes;
                Func<Type, IEnumerable<string>> getNames;
                if (genericResolver != null)
                {
                    candidateTypes = genericResolver.GetTypes();
                    getNames = genericResolver.GetNames;
                }
                else
                {
                    candidateTypes = ExportedTypeHelper.FromCurrentAppDomain(t => true);
                    getNames = t => new[] { t.Name };
                }
                foreach (var validType in candidateTypes)
                {
                    if (!MatchesConstraints(genericParameterAttributes, constraints, validType))
                    {
                        continue;
                    }
                    foreach (var templateName in getNames(validType))
                    {
                        var withTemplate = uriTemplate.Replace(templatePart, templateName);
                        routingTable.Add(withTemplate,
                                         new HandlerTypeInfo(exportedType.MakeGenericType(validType), respondsToTypes, respondsWithTypes));
                    }
                }
            }
        }

        private static bool MatchesConstraints(GenericParameterAttributes attributes, Type[] constraints, Type target)
        {
            if (constraints.Length == 0 && attributes == GenericParameterAttributes.None)
            {
                return true;
            }
            for (int i = 0; i < constraints.Length; i++)
            {
                if (!constraints[i].IsAssignableFrom(target))
                {
                    return false;
                }
            }
            if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            {
                if (target.GetConstructor(new Type[0]) == null)
                {
                    return false;
                }
            }
            if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
            {
                if (!(target.IsClass || target.IsInterface))
                {
                    return false;
                }
            }
            if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                if (!(target.IsValueType && !target.IsNullable()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}