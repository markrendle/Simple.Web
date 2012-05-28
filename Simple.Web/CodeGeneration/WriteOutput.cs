using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using ContentTypeHandling;
    using Links;

    static class WriteOutput
    {
        public static void Impl<T>(IOutput<T> handler, IContext context)
        {
            WriteUsingContentTypeHandler(handler, context);
        }

        private static void WriteUsingContentTypeHandler<T>(IOutput<T> handler, IContext context)
        {
            IContentTypeHandler contentTypeHandler;
            if (TryGetContentTypeHandler(context, out contentTypeHandler))
            {
                context.Response.ContentType = contentTypeHandler.GetContentType(context.Request.AcceptTypes);
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
    }
}
