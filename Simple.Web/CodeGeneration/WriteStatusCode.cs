using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using Http;

    internal static class WriteStatusCode
    {
        internal static void Impl(Status status, IContext context)
        {
            context.Response.Status = status.ToString();
        }
    }
}
