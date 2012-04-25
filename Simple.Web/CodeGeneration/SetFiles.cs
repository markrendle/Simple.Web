namespace Simple.Web.CodeGeneration
{
    using System.Linq;

    internal static class SetFiles
    {
        internal static void Impl(IUploadFiles endpoint, IContext context)
        {
            if (context.Request.Files.Any())
            {
                endpoint.Files = context.Request.Files;
            }
        }
    }
}