namespace Simple.Web.CodeGeneration
{
    using ContentTypeHandling;
    using Http;
    using Links;

    static class WriteView
    {
        public static void Impl(object handler, IContext context)
        {
            WriteUsingContentTypeHandler(handler, context);
        }

        private static void WriteUsingContentTypeHandler(object handler, IContext context)
        {
            if (context.Request.HttpMethod.Equals("HEAD")) return;
            IContentTypeHandler contentTypeHandler;
            if (TryGetContentTypeHandler(context, out contentTypeHandler))
            {
                context.Response.ContentType = contentTypeHandler.GetContentType(context.Request.AcceptTypes);

                var content = new Content(handler, null);
                contentTypeHandler.Write(content, context.Response.OutputStream);
            }
        }

        private static bool TryGetContentTypeHandler(IContext context, out IContentTypeHandler contentTypeHandler)
        {
            try
            {
                string matchedType;
                contentTypeHandler = new ContentTypeHandlerTable().GetContentTypeHandler(context.Request.AcceptTypes, out matchedType);
            }
            catch (UnsupportedMediaTypeException)
            {
                context.Response.StatusCode = 415;
                context.Response.StatusDescription = "Unsupported media type requested.";
                contentTypeHandler = null;
                return false;
            }
            return true;
        }
    }
}