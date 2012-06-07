using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;
    using Simple.Web.Authentication;
    using Simple.Web.Behaviors;

    [UriTemplate("/form")]
    public class GetForm : IGet, IRequireAuthentication
    {
        public string Title { get { return "Test Form"; } }

        public Status Get()
        {
            return 200;
        }

        public IUser CurrentUser { get; set; }
    }
}