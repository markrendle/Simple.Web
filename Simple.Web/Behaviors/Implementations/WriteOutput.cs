namespace Simple.Web.Behaviors.Implementations
{
    using System.Linq;
    using System.Text;
    using Behaviors;
    using CodeGeneration;
    using ContentTypeHandling;
    using Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class WriteOutput
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl<T>(IOutput<T> handler, IContext context)
        {
            if (typeof(T) == typeof(RawHtml))
            {
                WriteRawHtml((IOutput<RawHtml>)handler, context);
                return;
            }
            WriteUsingContentTypeHandler(handler, context);
        }

        private static void WriteUsingContentTypeHandler<T>(IOutput<T> handler, IContext context)
        {
            IContentTypeHandler contentTypeHandler;
            if (TryGetContentTypeHandler(context, out contentTypeHandler))
            {
                context.Response.ContentType = contentTypeHandler.GetContentType(context.Request.AcceptTypes);
                if (context.Request.HttpMethod.Equals("HEAD")) return;

                var content = new Content(handler, handler.Output);
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

        internal static void WriteRawHtml(IOutput<RawHtml> handler, IContext context)
        {
            context.Response.ContentType =
                context.Request.AcceptTypes.FirstOrDefault(
                    at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
            if (context.Request.HttpMethod.Equals("HEAD")) return;
            var bytes = Encoding.UTF8.GetBytes(handler.Output.ToString());
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }
    }
}
