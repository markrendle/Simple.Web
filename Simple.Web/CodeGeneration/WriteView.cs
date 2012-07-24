using System;

namespace Simple.Web.CodeGeneration
{
    using Helpers;
    using Http;
    using MediaTypeHandling;

    static class WriteView
    {
        public static void Impl(object handler, IContext context)
        {
            WriteUsingMediaTypeHandler(handler, context);
        }

        private static void WriteUsingMediaTypeHandler(object handler, IContext context)
        {
			if (context.Request.HttpMethod == null) throw new Exception("No HTTP Method given");
            if (context.Request.HttpMethod.Equals("HEAD")) return;
            IMediaTypeHandler mediaTypeHandler;
            if (TryGetMediaTypeHandler(context, out mediaTypeHandler))
            {
                context.Response.SetContentType(mediaTypeHandler.GetContentType(context.Request.Headers[HeaderKeys.Accept]));

                context.Response.WriteFunction = (stream, token) =>
                    {
                        var content = new Content(handler, null);
                        return mediaTypeHandler.Write(content, stream);
                    };
            }
        }

        private static bool TryGetMediaTypeHandler(IContext context, out IMediaTypeHandler mediaTypeHandler)
        {
            try
            {
                string matchedType;
                mediaTypeHandler = new MediaTypeHandlerTable().GetMediaTypeHandler(context.Request.GetAccept(), out matchedType);
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