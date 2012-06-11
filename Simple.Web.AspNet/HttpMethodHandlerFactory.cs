namespace Simple.Web.AspNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Authentication;
    using Helpers;
    using Hosting;
    using Http;
    using Routing;

    internal static class HttpMethodHandlerFactory
    {
        private static readonly ConcurrentDictionary<string, RoutingTable> RoutingTables = new ConcurrentDictionary<string, RoutingTable>(StringComparer.OrdinalIgnoreCase);

        private static RoutingTable BuildRoutingTable(string httpMethod)
        {
            var handlerTypes = ExportedTypeHelper.FromCurrentAppDomain(IsHttpMethodHandler)
                .Where(i => HttpMethodAttribute.Get(i).HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return new RoutingTableBuilder(handlerTypes).BuildRoutingTable();
        }

        private static bool IsHttpMethodHandler(Type type)
        {
            return HttpMethodAttribute.IsAppliedTo(type);
        }

        private static RoutingTable TableFor(string httpMethod)
        {
            return RoutingTables.GetOrAdd(httpMethod, BuildRoutingTable);
        }

        public static IHttpHandler TryCreate(IContext context)
        {
            // If it's OPTIONS, we can handle that without any help.
            if (context.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                return CreateOptionsHandler(context);
            }

            IDictionary<string, string> variables;
            var handlerType = TableFor(context.Request.HttpMethod).Get(context.Request.Url.AbsolutePath, context.Request.ContentType, context.Request.AcceptTypes, out variables);
            if (handlerType == null) return null;
            var handlerInfo = new HandlerInfo(handlerType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.Select(g => g.Key))
            {
                handlerInfo.Variables.Add(key, context.Request.QueryString[key].FirstOrDefault());
            }

            return CreateHandler(context, handlerInfo);
        }

        private static IHttpHandler CreateHandler(IContext context, HandlerInfo handlerInfo)
        {
            IHttpHandler instance;
            if (handlerInfo.IsAsync)
            {
                instance = CreateAsyncHttpHandler(context, handlerInfo);
            }
            else
            {
                instance = CreateHttpHandler(context, handlerInfo);
            }
            return instance;
        }

        private static IHttpHandler CreateHttpHandler(IContext context, HandlerInfo handlerInfo)
        {
            IHttpHandler instance;
            if (handlerInfo.RequiresAuthentication)
            {
                /*var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new DefaultAuthenticationProvider();*/
                instance = new SimpleHttpHandler(context, handlerInfo);
            }
            else
            {
                instance = new SimpleHttpHandler(context, handlerInfo);
            }
            return instance;
        }

        private static IHttpHandler CreateAsyncHttpHandler(IContext context, HandlerInfo handlerInfo)
        {
            IHttpHandler instance;
            if (handlerInfo.RequiresAuthentication)
            {
                var authenticationProvider = /*SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??*/
                                             new DefaultAuthenticationProvider();
                instance = new SimpleHttpAsyncHandler(context, handlerInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleHttpAsyncHandler(context, handlerInfo);
            }
            return instance;
        }

        private static IHttpHandler CreateOptionsHandler(IContext context)
        {
            return new OptionsHandler(RoutingTables);
        }
    }
}