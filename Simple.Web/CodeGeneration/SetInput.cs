using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using System.IO;
    using System.Reflection;

    internal static class SetInput
    {
        internal static void Impl<T>(IInput<T> endpoint, IContext context)
        {
            if (context.Request.InputStream.Length == 0) return;

            var contentTypeHandlerTable = new ContentTypeHandlerTable();
            var contentTypeHandler = contentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                endpoint.Input = (T)contentTypeHandler.Read(reader, typeof(T));
            }
        }
    }
}
