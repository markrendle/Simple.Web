using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    static class DisableCache
    {
        public static void Impl(ICacheability handler, IContext context)
        {
            if (handler.CacheOptions.Disable)
            {
                context.Response.DisableCache();
            }
        }
    }
}
