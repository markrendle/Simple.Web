using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    [UriTemplate("/form")]
    public class FormEndpoint : GetEndpoint<RawHtml>
    {
        protected override Status Get()
        {
            Output = Raw.Html(@"<html><body><form action=""/submit"" method=""POST""><input type=""text"" name=""Text"" /><input type=""submit"" /></form></body></html>");
            return 200;
        }
    }

    public class Form
    {
        public string Text { get; set; }
    }
}