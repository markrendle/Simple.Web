namespace Simple.Web.CodeGeneration
{
    using Simple.Web.Http;

    internal static class WriteStatusCode
    {
        internal static void Impl(Status status, IContext context)
        {
            context.Response.Status = status;
        }
    }
}