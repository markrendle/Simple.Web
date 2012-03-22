namespace Simple.Web.Razor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    public class MockEndpoint : IContent
    {
        public object Model { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Variables { get; set; }

        public string ViewPath
        {
            get { return null; }
        }
    }
}