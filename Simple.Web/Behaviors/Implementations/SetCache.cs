namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    static class SetCache
    {
        public static void Impl(ICacheability handler, IContext context)
        {
            if (handler.CacheOptions.Disable)
            {
                context.Response.DisableCache();
                return;
            }
            if (handler.CacheOptions.AbsoluteExpiry.HasValue)
            {
                context.Response.SetCacheAbsoluteExpiry(handler.CacheOptions.AbsoluteExpiry.Value);
            }
            else if (handler.CacheOptions.SlidingExpiry.HasValue)
            {
                context.Response.SetCacheSlidingExpiry(handler.CacheOptions.SlidingExpiry.Value);
            }
            else
            {
                return;
            }

            if (handler.CacheOptions.VaryByContentEncodings != null)
            {
                context.Response.SetCacheVaryByContentEncodings(handler.CacheOptions.VaryByContentEncodings);
            }
        }
    }
}
