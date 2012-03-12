namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    class GetHandler : HandlerBase
    {
        protected internal GetHandler() : base(typeof(GetEndpoint<>))
        {
        }

        internal GetHandler(params Type[] endpointTypes) : base(typeof(PostEndpoint<,>), endpointTypes)
        {
            
        }
    }
}