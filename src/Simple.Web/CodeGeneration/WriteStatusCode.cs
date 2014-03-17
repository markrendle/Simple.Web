using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using Http;

    using Simple.Web.DependencyInjection;

    internal static class WriteStatusCode
    {
        internal static void Impl(Status status, IContext context, ISimpleContainerScope container)
        {
            context.Response.Status = status;
        }
    }
}
