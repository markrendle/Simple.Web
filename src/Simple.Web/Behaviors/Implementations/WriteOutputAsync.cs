namespace Simple.Web.Behaviors.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Simple.Web.Helpers;
    using Simple.Web.Http;
    using Simple.Web.MediaTypeHandling;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class WriteOutputAsync
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <typeparam name="T">The output type.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> which will complete when the output has been written.</returns>
        public static Task Impl<T>(IOutputAsync<T> handler, IContext context)
        {
            if (typeof(T) == typeof(RawHtml))
            {
                return WriteRawHtml((IOutputAsync<RawHtml>)handler, context);
            }
            return WriteUsingMediaTypeHandler(handler, context);
        }

        internal static Task WriteRawHtml(IOutputAsync<RawHtml> handler, IContext context)
        {
            context.Response.SetContentType(
                context.Request.GetAccept().FirstOrDefault(at => at == MediaType.Html || at == MediaType.XHtml) ?? "text/html");
            if (context.Request.HttpMethod.Equals("HEAD"))
            {
                return TaskHelper.Completed();
            }

            context.Response.WriteFunction = stream =>
                                             {
                                                 var bytes = Encoding.UTF8.GetBytes(handler.Output.ToString());
                                                 return stream.WriteAsync(bytes, 0, bytes.Length);
                                             };
            return TaskHelper.Completed();
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

        private static Task WriteUsingMediaTypeHandler<T>(IOutputAsync<T> handler, IContext context)
        {
            IMediaTypeHandler mediaTypeHandler;
            var acceptedTypes = context.Request.GetAccept();
            if (TryGetMediaTypeHandler(context, acceptedTypes, out mediaTypeHandler))
            {
                context.Response.SetContentType(mediaTypeHandler.GetContentType(acceptedTypes));
                if (context.Request.HttpMethod.Equals("HEAD"))
                {
                    return TaskHelper.Completed();
                }

                context.Response.WriteFunction = stream => handler.Output.ContinueWith(t =>
                                                                                       {
                                                                                           var content = new Content(context.Request.Url,
                                                                                                                     handler,
                                                                                                                     t.Result);
                                                                                           return mediaTypeHandler.Write<T>(content, stream);
                                                                                       }).Unwrap();
            }
            return TaskHelper.Completed();
        }
    }
}