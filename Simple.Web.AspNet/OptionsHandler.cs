namespace Simple.Web.AspNet
{
    using System.Collections.Generic;
    using System.Web;
    using Routing;

    internal class OptionsHandler : IHttpHandler
    {
        private readonly IDictionary<string, RoutingTable> _routingTables;

        public OptionsHandler(IDictionary<string, RoutingTable> routingTables)
        {
            _routingTables = routingTables;
        }

        public void ProcessRequest(HttpContext context)
        {
            var allowed = new List<string>();
            foreach (var method in _routingTables.Keys)
            {
                IDictionary<string, string[]> _;
                if (_routingTables[method].Get(context.Request.Url.AbsolutePath, out _) != null)
                {
                    allowed.Add(method);
                }
            }

            context.Response.Headers["Allow"] = string.Join(",", allowed);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}