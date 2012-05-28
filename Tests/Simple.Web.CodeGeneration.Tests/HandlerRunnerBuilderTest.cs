namespace Simple.Web.CodeGeneration.Tests
{
    using Handlers;
    using Mocks;
    using Stubs;
    using Xunit;

    public class HandlerRunnerBuilderTest
    {
        [Fact]
        public void CallsAreMade()
        {
            Reset();
            var target = new HandlerRunnerBuilder(typeof (TestHandler), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestHandler(200), new MockContext());

            Assert.True(StubCheckAuthentication.Called);
            Assert.True(StubSetInput.Called);
            Assert.Equal(typeof(string), StubSetInput.WithType);
            Assert.True(StubWriteStatusCode.Called);
            Assert.True(StubSetResponseCookies.Called);
            Assert.True(StubSetInputETag.Called);
            Assert.True(StubSetOutputETag.Called);
            Assert.True(StubSetLastModified.Called);
            Assert.True(StubSetIfModifiedSince.Called);
        }
        
        [Fact]
        public void CallsAreMadeAsync()
        {
            Reset();
            var target = new HandlerRunnerBuilder(typeof (TestAsyncHandler), new StubMethodLookup());
            var runner = target.BuildAsyncRunner();
            var testAsyncHandler = new TestAsyncHandler(200);
            var mockContext = new MockContext();
            var task = runner.Start(testAsyncHandler, mockContext);
            task.Wait();
            runner.End(testAsyncHandler, mockContext, task.Result);

            Assert.True(StubCheckAuthentication.Called);
            Assert.True(StubSetInput.Called);
            Assert.Equal(typeof(string), StubSetInput.WithType);
            Assert.True(StubWriteStatusCode.Called);
            Assert.True(StubSetResponseCookies.Called);
            Assert.True(StubSetInputETag.Called);
            Assert.True(StubSetOutputETag.Called);
            Assert.True(StubSetLastModified.Called);
            Assert.True(StubSetIfModifiedSince.Called);
        }

        [Fact]
        public void RedirectPreventsFurtherCalls()
        {
            Reset();
            var target = new HandlerRunnerBuilder(typeof(TestRedirectHandler), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestRedirectHandler(301), new MockContext());

            Assert.True(StubRedirect.Called);
            Assert.False(StubWriteStreamResponse.Called);
        }

        [Fact]
        public void UnusedRedirectDoesNotPreventFurtherCalls()
        {
            Reset();
            var target = new HandlerRunnerBuilder(typeof(TestRedirectHandler), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestRedirectHandler(200), new MockContext());

            Assert.True(StubRedirect.Called);
            Assert.True(StubWriteStreamResponse.Called);
        }

        [Fact]
        public void UploadFilesCallSetFiles()
        {
            Reset();
            var target = new HandlerRunnerBuilder(typeof(TestUploadHandler), new StubMethodLookup());
            var runner = target.BuildRunner();
            runner(new TestUploadHandler(), new MockContext());

            Assert.True(StubSetFiles.Called);
        }

        private static void Reset()
        {
            StubCheckAuthentication.Called =
                StubSetInput.Called =
                StubWriteStatusCode.Called =
                StubSetResponseCookies.Called =
                StubRedirect.Called =
                StubWriteStreamResponse.Called =
                StubWriteOutput.Called =
                StubWriteRawHtml.Called =
                StubWriteView.Called =
                StubSetFiles.Called =
                StubSetCache.Called =
                false;

            StubSetInput.WithType = null;
            StubWriteOutput.WithType = null;
        }
    }
}
