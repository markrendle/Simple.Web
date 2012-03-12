namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    sealed class PostHandler : HandlerBase
    {
        internal PostHandler() : base(typeof(PostEndpoint<,>))
        {
        }

        internal PostHandler(IEnumerable<Type> endpointTypes) : base(typeof(PostEndpoint<,>), endpointTypes)
        {
            
        }

        protected override void OnRunning(IEndpoint endpoint, IContext context)
        {
            var inputEndpoint = (IInputEndpoint) endpoint;
            var contentTypeHandler = ContentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                inputEndpoint.Input = contentTypeHandler.Read(reader, inputEndpoint.InputType);
            }
        }
    }
}