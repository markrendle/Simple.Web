namespace Simple.Web.Tests
{
    using System;
    using System.Linq;
    using Behaviors;
    using CodeGeneration;
    using Hosting;
    using Http;
    using Mocks;
    using Xunit;

    public class ResponseHeaderTests
    {
        [Fact]
        public void SetsResponseHeaderUsingPropertyName()
        {
            var request = new MockRequest();
            var response = new MockResponse();
            var context = new MockContext { Request = request, Response = response };
            Run<SetResponseHeaderUsingPropertyNameHandler>(context);
            string[] header;
            Assert.True(response.Headers.TryGetValue("TestHeader", out header));
            Assert.True(header.Single() == "Pass");
        }

        [Fact]
        public void SetsResponseHeaderUsingSpecifiedName()
        {
            var request = new MockRequest();
            var response = new MockResponse();
            var context = new MockContext { Request = request, Response = response };
            Run<SetResponseHeaderUsingSpecifiedNameHandler>(context);
            string[] header;
            Assert.True(response.Headers.TryGetValue("X-Test-Header", out header));
            Assert.True(header.Single() == "Pass");
        }

        private static void Run<T>(IContext context)
        {
            var runner = new PipelineFunctionFactory(typeof(T)).BuildAsyncRunMethod("GET");
            var info = new HandlerInfo(typeof(T), "GET");
            try
            {
                runner(context, info).Wait();
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
        }
    }

    public class SetResponseHeaderUsingPropertyNameHandler : IGet
    {
        public Status Get()
        {
            TestHeader = "Pass";
            return 200;
        }

        [ResponseHeader]
        public string TestHeader { get; set; }
    }

    public class SetResponseHeaderUsingSpecifiedNameHandler : IGet
    {
        public Status Get()
        {
            TestHeader = "Pass";
            return 200;
        }

        [ResponseHeader("X-Test-Header")]
        public string TestHeader { get; set; }
    }
}