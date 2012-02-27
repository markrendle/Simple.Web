using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    public class IndexEndpoint : GetEndpoint
    {
        protected override string UriTemplate
        {
            get { return "/"; }
        }

        protected override object Run()
        {
            return "Simple.Web has entered the building.";
        }
    }
}