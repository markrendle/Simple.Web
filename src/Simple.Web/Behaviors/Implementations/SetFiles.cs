namespace Simple.Web.Behaviors.Implementations
{
    using System.Linq;
    using Behaviors;
    using Http;

    using Simple.Web.DependencyInjection;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetFiles
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <param name="container">The scoped container.</param>
        /// <returns></returns>
        public static void Impl(IUploadFiles handler, IContext context, ISimpleContainerScope container)
        {
            if (context.Request.Files.Any())
            {
                handler.Files = context.Request.Files;
            }
        }
    }
}