namespace Simple.Web.Behaviors.Implementations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Helpers;
    using Http;
    using MediaTypeHandling;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class WriteOutputEnumerableAsync
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <typeparam name="T">The output type.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> which will complete when the output has been written.</returns>
        public static Task Impl<T>(IOutputEnumerableAsync<T> handler, IContext context)
        {
            return WriteUsingMediaTypeHandler(handler, context);
        }

        private static Task WriteUsingMediaTypeHandler<T>(IOutputEnumerableAsync<T> handler, IContext context)
        {
            IMediaTypeHandler mediaTypeHandler;
            var acceptedTypes = context.Request.GetAccept();
            if (TryGetMediaTypeHandler(context, acceptedTypes, out mediaTypeHandler))
            {
                context.Response.SetContentType(mediaTypeHandler.GetContentType(acceptedTypes));
                if (context.Request.HttpMethod.Equals("HEAD")) return TaskHelper.Completed();

                context.Response.WriteFunction = (stream) => handler.Output.ContinueWith(t =>
                    {
                        var content = new Content(context.Request.Url, handler, t.Result);
                        return mediaTypeHandler.Write<T>(content, stream);
                    }).Unwrap();
            }
            return TaskHelper.Completed();
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