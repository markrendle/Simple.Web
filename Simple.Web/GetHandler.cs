namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    class GetHandler : HandlerBase<IGet>
    {
        protected internal GetHandler() : base()
        {
        }

        internal GetHandler(params Type[] endpointTypes) : base(endpointTypes)
        {
            
        }
    }
}