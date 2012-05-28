namespace Simple.Web.CodeGeneration
{
    using System.Web;

    internal static class SetLastModified
    {
        internal static void Impl(IModified handler, IContext context)
        {
            if (handler.LastModified.HasValue)
            {
                context.Response.SetLastModified(handler.LastModified.Value);
            }
        }
    }
}