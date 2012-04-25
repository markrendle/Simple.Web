namespace Simple.Web.CodeGeneration.Tests
{
    using Endpoints;
    using Mocks;
    using Stubs;
    using Xunit;

    public class EndpointRunnerFactoryTest
    {
        [Fact]
        public void CallsAreMade()
        {
            Reset();
            var target = new EndpointRunnerBuilder(typeof (TestEndpoint), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestEndpoint(200), new MockContext());

            Assert.True(StubCheckAuthentication.Called);
            Assert.True(StubSetInput.Called);
            Assert.Equal(typeof(string), StubSetInput.WithType);
            Assert.True(StubWriteStatusCode.Called);
            Assert.True(StubSetCookies.Called);
        }

        [Fact]
        public void RedirectPreventsFurtherCalls()
        {
            Reset();
            var target = new EndpointRunnerBuilder(typeof(TestRedirectEndpoint), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestRedirectEndpoint(301), new MockContext());

            Assert.True(StubRedirect.Called);
            Assert.False(StubWriteStreamResponse.Called);
        }

        [Fact]
        public void UnusedRedirectDoesNotPreventFurtherCalls()
        {
            Reset();
            var target = new EndpointRunnerBuilder(typeof(TestRedirectEndpoint), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestRedirectEndpoint(200), new MockContext());

            Assert.True(StubRedirect.Called);
            Assert.True(StubWriteStreamResponse.Called);
        }

        [Fact]
        public void UploadFilesCallSetFiles()
        {
            Reset();
            var target = new EndpointRunnerBuilder(typeof(TestUploadEndpoint), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestUploadEndpoint(), new MockContext());

            Assert.True(StubSetFiles.Called);
        }

        private static void Reset()
        {
            StubCheckAuthentication.Called =
                StubSetInput.Called =
                StubWriteStatusCode.Called =
                StubSetCookies.Called =
                StubRedirect.Called =
                StubWriteStreamResponse.Called =
                StubWriteOutput.Called =
                StubWriteRawHtml.Called =
                StubWriteView.Called =
                StubSetFiles.Called =
                StubDisableCache.Called =
                false;

            StubSetInput.WithType = null;
            StubWriteOutput.WithType = null;
        }
    }
}
