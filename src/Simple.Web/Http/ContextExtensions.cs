namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Simple.Web.Cors;

    public static class ContextExtensions
    {
        public static void SetAccessControlHeaders(this IContext context, IList<IAccessControlEntry> accessControl)
        {
            if (accessControl == null || accessControl.Count == 0)
            {
                return;
            }

            // For wildcards just add it
            if (accessControl.Count == 1 && accessControl[0].Origin == "*")
            {
                context.Response.SetAccessControl(accessControl[0]);
                return;
            }

            // If there's no Origin header in the request, no headers needed
            var origin = context.Request.GetOrigin();
            if (string.IsNullOrWhiteSpace(origin))
            {
                return;
            }

            // If origin and host match we don't need headers
            var host = context.Request.Host;
            if (new Uri(origin).Authority.Equals(host, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Find a matching entry for the request origin
            var entry = accessControl.FirstOrDefault(e => e.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase));
            if (entry != null)
            {
                context.Response.SetAccessControl(entry);
            }
        }
    }
}