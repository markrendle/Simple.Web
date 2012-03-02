using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    public class IndexEndpoint : GetEndpoint<string>
    {
        public override string UriTemplate
        {
            get { return "/"; }
        }

        protected override string Get()
        {
            return "Simple.Web has entered the building.";
        }
    }
}