namespace Simple.Web.CodeGeneration
{
    internal static class SetOutputETag
    {
        internal static void Impl(IETag handler, IContext context)
        {
            if (!string.IsNullOrWhiteSpace(handler.OutputETag))
            {
                context.Response.SetETag(handler.OutputETag);
            }
        }
    }
}