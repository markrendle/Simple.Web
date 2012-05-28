using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using System.IO;
    using System.Reflection;
    using ContentTypeHandling;

    internal static class SetInput
    {
        internal static void Impl<T>(IInput<T> handler, IContext context)
        {
            if (context.Request.InputStream.Length == 0) return;

            var contentTypeHandlerTable = new ContentTypeHandlerTable();
            var contentTypeHandler = contentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            handler.Input = (T)contentTypeHandler.Read(context.Request.InputStream, typeof(T));
        }
    }
}
