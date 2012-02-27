using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using Simple.Web;

    public class FormEndpoint : GetEndpoint
    {
        protected override string UriTemplate
        {
            get { return "/form"; }
        }

        protected override object Run()
        {
            return @"<html><body><form action=""/submit"" method=""POST""><input type=""text"" name=""Text"" /><input type=""submit"" /></form></body></html>";
        }
    }

    public class SubmitEndpoint : PostEndpoint<Form>
    {
        protected override string UriTemplate
        {
            get { return "/submit"; }
        }

        protected override object Run()
        {
            return "Posted!";
        }
    }

    public class Form
    {
        public string Text { get; set; }
    }
}