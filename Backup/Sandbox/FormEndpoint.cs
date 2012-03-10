using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    public class FormEndpoint : GetEndpoint<string>
    {
        public override string UriTemplate
        {
            get { return "/form"; }
        }

        protected override string Get()
        {
            return @"<html><body><form action=""/submit"" method=""POST""><input type=""text"" name=""Text"" /><input type=""submit"" /></form></body></html>";
        }
    }

    public class Form
    {
        public string Text { get; set; }
    }
}