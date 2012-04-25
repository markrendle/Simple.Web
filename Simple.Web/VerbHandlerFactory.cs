namespace Simple.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    internal class VerbHandlerFactory<TSync,TAsync>
    {
        private static readonly ConcurrentDictionary<string, RoutingTable> RoutingTables = new ConcurrentDictionary<string, RoutingTable>(StringComparer.OrdinalIgnoreCase);

        private static RoutingTable BuildRoutingTable(string verb)
        {
            var handlerTypes = ExportedTypeHelper.FromCurrentAppDomain(IsVerbHandler)
                .Where(i => HttpVerbAttribute.Get(i).Verb.Equals(verb, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return new RoutingTableBuilder(handlerTypes).BuildRoutingTable();
        }

        private static bool IsVerbHandler(Type type)
        {
            return HttpVerbAttribute.IsAppliedTo(type);
        }

        private static RoutingTable TableFor(string verb)
        {
            return RoutingTables.GetOrAdd(verb, BuildRoutingTable);
        }

        public static IHttpHandler TryCreate(IContext context)
        {
            IDictionary<string, string> variables;
            var endpointType = TableFor(context.Request.HttpMethod).Get(context.Request.Url.AbsolutePath, context.Request.AcceptTypes, out variables);
            if (endpointType == null) return null;
            var endpointInfo = new EndpointInfo(endpointType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                endpointInfo.Variables.Add(key, context.Request.QueryString[key]);
            }

            return CreateHandler(context, endpointInfo);
        }

        private static IHttpHandler CreateHandler(IContext context, EndpointInfo endpointInfo)
        {
            IHttpHandler instance;
            if (typeof (TSync).IsAssignableFrom(endpointInfo.EndpointType))
            {
                instance = CreateHttpHandler(context, endpointInfo);
            }
            else if (typeof (TAsync).IsAssignableFrom(endpointInfo.EndpointType))
            {
                instance = CreateAsyncHttpHandler(context, endpointInfo);
            }
            else
            {
                throw new RoutingException(context.Request.Url.ToString(), "No matching route found.");
            }
            return instance;
        }

        private static IHttpHandler CreateHttpHandler(IContext context, EndpointInfo endpointInfo)
        {
            IHttpHandler instance;
            if (endpointInfo.RequiresAuthentication)
            {
                var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new AuthenticationProvider();
                instance = new SimpleHandler(context, endpointInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleHandler(context, endpointInfo);
            }
            return instance;
        }

        private static IHttpHandler CreateAsyncHttpHandler(IContext context, EndpointInfo endpointInfo)
        {
            IHttpHandler instance;
            if (endpointInfo.RequiresAuthentication)
            {
                var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new AuthenticationProvider();
                instance = new SimpleAsyncHandler<TAsync>(context, endpointInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleAsyncHandler<TAsync>(context, endpointInfo);
            }
            return instance;
        }
    }
}