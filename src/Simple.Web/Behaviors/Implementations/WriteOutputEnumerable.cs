namespace Simple.Web.Behaviors.Implementations
{
    using System.Collections.Generic;
    using Behaviors;
    using Http;
    using MediaTypeHandling;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class WriteOutputEnumerable
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <typeparam name="T">The output model type.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        public static void Impl<T>(IOutputEnumerable<T> handler, IContext context)
        {
            if (typeof (T).IsClass)
            {
                if (ReferenceEquals(handler.Output, null)) return;
            }
            WriteUsingMediaTypeHandler(handler, context);
        }

        private static void WriteUsingMediaTypeHandler<T>(IOutputEnumerable<T> handler, IContext context)
        {
            IMediaTypeHandler mediaTypeHandler;
            var acceptedTypes = context.Request.GetAccept();
            if (TryGetMediaTypeHandler(context, acceptedTypes, out mediaTypeHandler))
            {
                context.Response.SetContentType(mediaTypeHandler.GetContentType(acceptedTypes));
                if (context.Request.HttpMethod.Equals("HEAD")) return;

                context.Response.WriteFunction = (stream) =>
                    {
                        var content = new Content(context.Request.Url, handler, handler.Output);
                        return mediaTypeHandler.Write<T>(content, stream);
                    };
            }
        }

        private static bool TryGetMediaTypeHandler(IContext context, IList<string> acceptedTypes,
                                                   out IMediaTypeHandler mediaTypeHandler)
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
    }
}