namespace Simple.Web.Razor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using ContentTypeHandling;
    using Links;

    public class MockHandler : IContent
    {
        public object Handler { get; set; }

        public object Model { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Variables { get; set; }
        public string Title { get; set; }

        public string ViewPath
        {
            get { return null; }
        }

        public IEnumerable<Link> Links
        {
            get { return Enumerable.Empty<Link>(); }
        }
    }
}