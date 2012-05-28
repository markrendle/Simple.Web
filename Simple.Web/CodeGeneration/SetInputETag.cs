namespace Simple.Web.CodeGeneration
{
    internal static class SetInputETag
    {
        internal static void Impl(IETag handler, IContext context)
        {
            var etag = context.Request.Headers.Get("ETag");
            if (!string.IsNullOrWhiteSpace(etag))
            {
                handler.InputETag = etag;
            }
        }
    }
}