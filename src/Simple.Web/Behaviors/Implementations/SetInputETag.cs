namespace Simple.Web.Behaviors.Implementations
{
    using System.Linq;

    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetInputETag
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(IETag handler, IContext context)
        {
            if (!context.Request.Headers.ContainsKey("ETag"))
            {
                return;
            }
            var etag = context.Request.Headers["ETag"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(etag))
            {
                handler.InputETag = etag;
            }
        }
    }
}