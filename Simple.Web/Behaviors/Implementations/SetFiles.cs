namespace Simple.Web.Behaviors.Implementations
{
    using System.Linq;
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class SetFiles
    {
        public static void Impl(IUploadFiles handler, IContext context)
        {
            if (context.Request.Files.Any())
            {
                handler.Files = context.Request.Files;
            }
        }
    }
}