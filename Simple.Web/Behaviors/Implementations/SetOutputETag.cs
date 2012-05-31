namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class SetOutputETag
    {
        public static void Impl(IETag handler, IContext context)
        {
            if (!string.IsNullOrWhiteSpace(handler.OutputETag))
            {
                context.Response.SetETag(handler.OutputETag);
            }
        }
    }
}