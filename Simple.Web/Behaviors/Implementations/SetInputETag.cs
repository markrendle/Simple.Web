namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class SetInputETag
    {
        public static void Impl(IETag handler, IContext context)
        {
            var etag = context.Request.Headers.Get("ETag");
            if (!string.IsNullOrWhiteSpace(etag))
            {
                handler.InputETag = etag;
            }
        }
    }
}