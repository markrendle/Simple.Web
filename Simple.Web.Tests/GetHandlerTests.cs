namespace Simple.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using Mocks;
    using Xunit;

    public class GetHandlerTests
    {
        [Fact]
        public void GetRootWithHtmlReturnsHtml()
        {
            var target = new GetHandler(typeof(RootEndpoint));
            var strb = new StringBuilder();
            var context = new MockContext
                              {
                                  Request = new MockRequest
                                                {
                                                    Url = new Uri("http://test.com/"),
                                                    AcceptTypes = new[] { "text/html" },
                                                    HttpMethod = "GET",
                                                },
                                  Response = new MockResponse
                                                 {
                                                     Output = new StringWriter(strb)
                                                 }
                              };
            target.HandleRequest(context);
            Assert.Equal(200, context.Response.StatusCode);
            Assert.Equal("text/html", context.Response.ContentType);
            Assert.NotEqual(0, strb.Length);
        }
    }

    

    [UriTemplate("/")]
    [RespondsTo(ContentType.Html)]
    public class RootEndpoint : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            return Status.OK;
        }

        public RawHtml Output { get { return "<h1>Hello</h1>"; } }
    }
}
