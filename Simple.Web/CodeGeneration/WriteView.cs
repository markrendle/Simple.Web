using System;

namespace Simple.Web.CodeGeneration
{
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
                context.Response.ContentType = mediaTypeHandler.GetContentType(context.Request.AcceptTypes);

                var content = new Content(handler, null);
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
    }
}