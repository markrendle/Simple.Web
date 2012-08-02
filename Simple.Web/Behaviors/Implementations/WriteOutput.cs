namespace Simple.Web.Behaviors.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Behaviors;
    using CodeGeneration;
    using Helpers;
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
        /// <typeparam name="T">The output model type.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
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
            var acceptedTypes = context.Request.GetAccept();
            if (TryGetMediaTypeHandler(context, acceptedTypes, out mediaTypeHandler))
            {
                context.Response.SetContentType(mediaTypeHandler.GetContentType(acceptedTypes));
                if (context.Request.HttpMethod.Equals("HEAD")) return;

                context.Response.WriteFunction = (stream) =>
                    {
                        var content = new Content(handler, handler.Output);
                        return mediaTypeHandler.Write(content, stream);
                    };
            }
        }

        private static bool TryGetMediaTypeHandler(IContext context, IList<string> acceptedTypes, out IMediaTypeHandler mediaTypeHandler)
        {
            try
            {
                string matchedType;
                mediaTypeHandler = new MediaTypeHandlerTable().GetMediaTypeHandler(acceptedTypes, out matchedType);
            }
            catch (UnsupportedMediaTypeException)
            {
                context.Response.Status = "415 Unsupported media type requested.";
                mediaTypeHandler = null;
                return false;
            }
            return true;
        }

        internal static void WriteRawHtml(IOutput<RawHtml> handler, IContext context)
        {
            context.Response.SetContentType(GetHtmlContentType(context));
            if (context.Request.HttpMethod.Equals("HEAD")) return;
            context.Response.WriteFunction = (stream) =>
                {
                    var bytes = Encoding.UTF8.GetBytes(handler.Output.ToString());
                    return stream.WriteAsync(bytes, 0, bytes.Length);
                };
        }

        private static string GetHtmlContentType(IContext context)
        {
            return context.Request.GetAccept()
                   .FirstOrDefault( at => at == MediaType.Html || at == MediaType.XHtml) ?? "text/html";
        }
    }
}
