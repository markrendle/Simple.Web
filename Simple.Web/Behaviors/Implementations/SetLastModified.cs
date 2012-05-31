namespace Simple.Web.Behaviors.Implementations
{
    using Behaviors;
    using Http;

    public static class SetLastModified
    {
        public static void Impl(IModified handler, IContext context)
        {
            if (handler.LastModified.HasValue)
            {
                context.Response.SetLastModified(handler.LastModified.Value);
            }
        }
    }
}