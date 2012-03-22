using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    [UriTemplate("/")]
    public class GetIndex : IGet
    {
        public Status Get()
        {
            return Status.OK;
        }

        public object Output
        {
            get { return Raw.Html("Simple.Web has entered the building."); }
        }
    }
}