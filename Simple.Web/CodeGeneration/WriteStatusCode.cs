using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    internal static class WriteStatusCode
    {
        internal static void Impl(Status status, IContext context)
        {
            context.Response.StatusCode = status.Code;
            context.Response.StatusDescription = status.Description;
        }
    }
}
