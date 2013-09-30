namespace Simple.Web.Razor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Simple.Web.Links;
    using Simple.Web.MediaTypeHandling;

    public class MockHandler : IContent
    {
        public object Handler { get; set; }

        public IEnumerable<Link> Links
        {
            get { return Enumerable.Empty<Link>(); }
        }

        public object Model { get; set; }

        public string Title { get; set; }

        public Uri Uri { get; private set; }

        public IEnumerable<KeyValuePair<string, object>> Variables { get; set; }

        public string ViewPath
        {
            get { return null; }
        }
    }
}