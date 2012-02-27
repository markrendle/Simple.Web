namespace Simple.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Placeholder. Coolness will go here.
    /// </summary>
    public class SimpleWebHttpHandler : IHttpHandler
    {
        private static readonly object RoutingTableLock = new object();
        private static volatile RoutingTable _routingTable;

        public void ProcessRequest(HttpContext context)
        {
            if (_routingTable == null)
            {
                lock (RoutingTableLock)
                {
                    if (_routingTable == null)
                    {
                        var routingTable = new RoutingTable();
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            foreach (var exportedType in assembly.GetExportedTypes().Where(type => typeof(GetEndpoint).IsAssignableFrom(type) && !type.IsAbstract))
                            {
                                var instance = Activator.CreateInstance(exportedType) as GetEndpoint;
                                if (instance != null)
                                {
                                    routingTable.Add(instance.UriTemplate, exportedType);
                                }
                            }
                        }

                        _routingTable = routingTable;
                    }
                }
            }

            IDictionary<string,string> variables;
            var endpointType = _routingTable.Get(context.Request.Url.AbsolutePath, out variables);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                variables.Add(key, context.Request.QueryString[key]);
            }

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

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
