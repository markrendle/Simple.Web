namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetCache
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
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
