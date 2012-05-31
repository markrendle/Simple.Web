namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.ContentTypeHandling;
    using Simple.Web.Http;

    public static class SetInput
    {
        public static void Impl<T>(IInput<T> handler, IContext context)
        {
            if (context.Request.InputStream.Length == 0) return;

            var contentTypeHandlerTable = new ContentTypeHandlerTable();
            var contentTypeHandler = contentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            handler.Input = (T)contentTypeHandler.Read(context.Request.InputStream, typeof(T));
        }
    }
}
