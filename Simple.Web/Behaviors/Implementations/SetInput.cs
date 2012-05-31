namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.ContentTypeHandling;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetInput
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl<T>(IInput<T> handler, IContext context)
        {
            if (context.Request.InputStream.Length == 0) return;

            var contentTypeHandlerTable = new ContentTypeHandlerTable();
            var contentTypeHandler = contentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            handler.Input = (T)contentTypeHandler.Read(context.Request.InputStream, typeof(T));
        }
    }
}
