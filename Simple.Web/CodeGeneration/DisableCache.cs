using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    static class DisableCache
    {
        public static void Impl(IContext context)
        {
            context.Response.DisableCache();
        }
    }
}
