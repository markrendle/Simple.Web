namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Http;
    using Simple.Web.MediaTypeHandling;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetInput
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <typeparam name="T">The input model type.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        public static void Impl<T>(IInput<T> handler, IContext context)
        {
            if (context.Request.InputStream.CanSeek && context.Request.InputStream.Length == 0)
            {
                return;
            }

            var mediaTypeHandlerTable = new MediaTypeHandlerTable();
            var mediaTypeHandler = mediaTypeHandlerTable.GetMediaTypeHandler(context.Request.GetContentType());
            handler.Input = mediaTypeHandler.Read<T>(context.Request.InputStream).Result;
        }
    }
}