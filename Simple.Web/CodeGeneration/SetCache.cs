using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using System.Web;

    static class SetCache
    {
        public static void Impl(ICacheability handler, IContext context)
        {
            if (handler.CacheOptions.Disable)
            {
                context.Response.DisableCache();
            }
            else if (handler.CacheOptions.AbsoluteExpiry.HasValue)
            {
                context.Response.SetCacheAbsoluteExpiry(handler.CacheOptions.AbsoluteExpiry.Value);
            }
            else if (handler.CacheOptions.SlidingExpiry.HasValue)
            {
                context.Response.SetCacheSlidingExpiry(handler.CacheOptions.SlidingExpiry.Value);
            }
        }
    }
}
