namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetCacheOptions
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(ICacheability handler, IContext context)
        {
            CacheOptions cacheOptions;
            if ((cacheOptions = handler.CacheOptions) == null) return;

            context.Response.SetCacheOptions(cacheOptions);
        }
    }
}
