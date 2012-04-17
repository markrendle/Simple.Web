namespace Simple.Web
{
    using System.IO;
    using System.Linq;

    static class ResponseWriter
    {
        private static readonly ContentTypeHandlerTable HandlerTable = new ContentTypeHandlerTable();

        public static void Write(IEndpointRunner runner, IContext context)
        {
            var hasOutput = runner.HasOutput;

            if (hasOutput && runner.Output is RawHtml)
            {
                WriteRawHtml(runner, context);
            }
            else if (hasOutput && runner.Output is Stream)
            {
                CopyStreamToResponse(runner, context);
            }
            else if (hasOutput || runner.Endpoint is ISpecifyView)
            {
                WriteUsingContentTypeHandler(runner, context);
            }
        }

        private static void WriteRawHtml(IEndpointRunner runner, IContext context)
        {
            context.Response.ContentType =
                context.Request.AcceptTypes.FirstOrDefault(
                    at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
            context.Response.Output.Write(runner.Output.ToString());
        }

        private static void CopyStreamToResponse(IEndpointRunner runner, IContext context)
        {
            var outputStream = runner.Endpoint as IOutputStream;
            if (outputStream != null)
            {
                context.Response.ContentType = outputStream.ContentType;
                if (!string.IsNullOrWhiteSpace(outputStream.ContentDisposition))
                {
                    context.Response.Headers.Set("Content-Disposition", outputStream.ContentDisposition);
                }
            }
            using (var stream = (Stream)runner.Output)
            {
                stream.Position = 0;
                stream.CopyTo(context.Response.OutputStream);
            }
        }

        private static void WriteUsingContentTypeHandler(IEndpointRunner runner, IContext context)
        {
            IContentTypeHandler contentTypeHandler;
            if (TryGetContentTypeHandler(context, out contentTypeHandler))
            {
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
                context.Response.StatusDescription = "Unsupported media type requested.";
                contentTypeHandler = null;
                return false;
            }
            return true;
        }
    }
}