namespace Simple.Web.Behaviors.Implementations
{
    using System.Linq;
    using System.Text;
    using Behaviors;
    using CodeGeneration;
    using Http;
    using MediaTypeHandling;

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
            WriteUsingMediaTypeHandler(handler, context);
        }

        private static void WriteUsingMediaTypeHandler<T>(IOutput<T> handler, IContext context)
        {
            IMediaTypeHandler mediaTypeHandler;
            if (TryGetMediaTypeHandler(context, out mediaTypeHandler))
            {
                context.Response.ContentType = mediaTypeHandler.GetContentType(context.Request.AcceptTypes);
                if (context.Request.HttpMethod.Equals("HEAD")) return;

                var content = new Content(handler, handler.Output);
                mediaTypeHandler.Write(content, context.Response.OutputStream);
            }
        }

        private static bool TryGetMediaTypeHandler(IContext context, out IMediaTypeHandler mediaTypeHandler)
        {
            try
            {
                string matchedType;
                mediaTypeHandler = new MediaTypeHandlerTable().GetMediaTypeHandler(context.Request.AcceptTypes, out matchedType);
            }
            catch (UnsupportedMediaTypeException)
            {
                context.Response.StatusCode = 415;
                context.Response.StatusDescription = "Unsupported media type requested.";
                mediaTypeHandler = null;
                return false;
            }
            return true;
        }

        internal static void WriteRawHtml(IOutput<RawHtml> handler, IContext context)
        {
            context.Response.ContentType =
                context.Request.AcceptTypes.FirstOrDefault(
                    at => at == MediaType.Html || at == MediaType.XHtml) ?? "text/html";
            if (context.Request.HttpMethod.Equals("HEAD")) return;
            var bytes = Encoding.UTF8.GetBytes(handler.Output.ToString());
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }
    }
}
