using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    [UriTemplate("/")]
    public class IndexEndpoint : GetEndpoint<RawHtml>
    {
        protected override Status Get()
        {
            Output = Raw.Html("Simple.Web has entered the building.");
            return Status.OK;
        }
    }
}