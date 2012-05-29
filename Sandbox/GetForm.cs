using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

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

    public class Form
    {
        public string Text { get; set; }
    }
}