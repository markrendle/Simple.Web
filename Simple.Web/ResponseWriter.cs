namespace Simple.Web
{
    using System.IO;
    using System.Linq;

    static class ResponseWriter
    {
        private static readonly ContentTypeHandlerTable HandlerTable = new ContentTypeHandlerTable();

        public static void Write(IEndpointRunner runner, IContext context)
        {
            if (runner.HasOutput && runner.Output is RawHtml)
            {
                context.Response.ContentType =
                    context.Request.AcceptTypes.FirstOrDefault(
                        at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
                context.Response.Output.Write(runner.Output.ToString());
            }
            else if (runner.HasOutput && runner.Output is Stream)
            {
                var outputStream = runner.Endpoint as IOutputStream;
                if (outputStream != null)
                {
                    context.Response.ContentType = outputStream.ContentType;
                }
                ((Stream)runner.Output).CopyTo(context.Response.OutputStream);
            }
            else
            {
                IContentTypeHandler contentTypeHandler;
                if (!TryGetContentTypeHandler(context, out contentTypeHandler))
                {
                    throw new UnsupportedMediaTypeException(context.Request.AcceptTypes);
                }
                context.Response.ContentType = contentTypeHandler.GetContentType(context.Request.AcceptTypes);
                contentTypeHandler.Write(new Content(runner), context.Response.Output);
            }
        }

        private static bool TryGetContentTypeHandler(IContext context, out IContentTypeHandler contentTypeHandler)
        {
            try
            {
                contentTypeHandler = HandlerTable.GetContentTypeHandler(context.Request.AcceptTypes);
            }
            catch (UnsupportedMediaTypeException)
            {
                context.Response.StatusCode = 415;
                context.Response.Close();
                contentTypeHandler = null;
                return false;
            }
            return true;
        }
    }
}