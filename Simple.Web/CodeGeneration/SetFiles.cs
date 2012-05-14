namespace Simple.Web.CodeGeneration
{
    using System.Linq;

    internal static class SetFiles
    {
        internal static void Impl(IUploadFiles handler, IContext context)
        {
            if (context.Request.Files.Any())
            {
                handler.Files = context.Request.Files;
            }
        }
    }
}