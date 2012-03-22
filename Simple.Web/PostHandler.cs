namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    sealed class PostHandler : HandlerBase<IPost>
    {
        internal PostHandler() : base()
        {
        }

        internal PostHandler(IEnumerable<Type> endpointTypes) : base(endpointTypes)
        {
            
        }

        protected override void OnRunning(EndpointRunner endpoint, IContext context)
        {
            if (!endpoint.HasInput) return;

            var contentTypeHandler = ContentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                endpoint.Input = contentTypeHandler.Read(reader, endpoint.InputType);
            }
        }
    }
}