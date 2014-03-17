namespace Simple.Web.Tests.ApplicationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Simple.Web.Behaviors;
    using Simple.Web.DependencyInjection;
    using Simple.Web.Http;
    using Simple.Web.TestHelpers;

    using Xunit;

    public class ApplicationTests
    {
        private readonly IDictionary<string, object> context;

        public ApplicationTests()
        {
            var streamRequest = new NonClosingMemoryStream(new MemoryStream());
            var streamResponse = new NonClosingMemoryStream(new MemoryStream());

            this.context = new Dictionary<string, object>
                                                    {
                                                        { "host.AppName", "TestApp" },
                                                        { "server.RemoteIpAddress", "1.2.3.4" },
                                                        { "owin.CallCancelled", new CancellationToken() },
                                                        { "owin.RequestProtocol", "HTTP" },
                                                        { "owin.RequestMethod", "GET" },
                                                        { "owin.RequestBody", (Stream)streamRequest },
                                                        { "owin.RequestPath", "" },
                                                        { "owin.RequestQueryString", string.Empty },
                                                        { "owin.ResponseHeaders", new Dictionary<string, string[]>() },
                                                        { "owin.ResponseBody", (Stream)streamResponse },
                                                        { "owin.RequestHeaders", new Dictionary<string, string[]>
                                                                                     {
                                                                                         { "X-Something", new [] { "somevalue" } }
                                                                                     }}};
        }

        //[Fact]
        //public void HandlerExceptionReturnsFaultedTask()
        //{
        //    this.context["owin.RequestPath"] = "/some/exception";

        //    var task = Application.Run(this.context)
        //        .ContinueWith(
        //        t =>
        //        {
        //            Assert.IsAssignableFrom<Task>(t);
        //            Assert.True(t.IsFaulted);
        //            Assert.False(t.IsCanceled);
        //            Assert.NotNull(t.Exception);
        //            Assert.Equal(ExceptionEndpoint.ExceptionMessage, t.Exception.Message);
        //        },
        //            TaskContinuationOptions.OnlyOnRanToCompletion);

        //    task.Wait();
        //}

        //[Fact]
        //public void BehaviorWrappedHandlerExceptionReturnsFaultedTask()
        //{
        //    this.context["owin.RequestPath"] = "/some/behavior/exception";

        //    var task = Application.Run(this.context)
        //        .ContinueWith(
        //        t =>
        //        {
        //            Assert.IsAssignableFrom<Task>(t);
        //            Assert.True(t.IsFaulted);
        //            Assert.False(t.IsCanceled);
        //            Assert.NotNull(t.Exception);
        //            Assert.Equal(BehaviorExceptionEndpoint.ExceptionMessage, t.Exception.Message);
        //        },
        //            TaskContinuationOptions.OnlyOnRanToCompletion);

        //    task.Wait();
        //}

        [Fact]
        public void FourOFourReturnsResult()
        {
            this.context["owin.RequestPath"] = "/some/behavior/404";

            var task = Application.Run(this.context)
                .ContinueWith(
                t =>
                {
                    Assert.IsAssignableFrom<Task>(t);
                    Assert.False(t.IsFaulted);
                    Assert.False(t.IsCanceled);
                    Assert.Null(t.Exception);
                    Assert.Equal(404, this.context["owin.ResponseStatusCode"]);
                },
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            task.Wait();
        }

        [Fact]
        public void LongRunningWaitsForComletion()
        {
            this.context["owin.RequestPath"] = "/some/long/running";

            var task = Application.Run(this.context)
                .ContinueWith(
                t =>
                {
                    Assert.IsAssignableFrom<Task>(t);
                    Assert.False(t.IsFaulted);
                    Assert.False(t.IsCanceled);
                    Assert.Null(t.Exception);
                    Assert.Equal(200, this.context["owin.ResponseStatusCode"]);
                },
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            task.Wait();
        }

        [Fact]
        public void SomeOutputIsReturned()
        {
            string result = null;

            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                this.context["owin.ResponseBody"] = stream;
                this.context["owin.RequestPath"] = "/some/output/returned";

                var task = Application.Run(this.context)
                    .ContinueWith(t =>
                    {
                        Assert.IsAssignableFrom<Task>(t);
                        Assert.False(t.IsFaulted);
                        Assert.False(t.IsCanceled);
                        Assert.Null(t.Exception);
                        Assert.Equal(200, this.context["owin.ResponseStatusCode"]);

                        stream.Position = 0;

                        using (var reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }

                        stream.ForceDispose();

                        Assert.Equal("Testing 1 2 3", result);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                task.Wait();
            }
        }
    }

    [UriTemplate("/some/exception")]
    public class ExceptionEndpoint : IGet
    {
        public const string ExceptionMessage = "This should result in a faulted Task exception.";

        public Status Get()
        {
            throw new Exception(ExceptionMessage);
        }
    }

    [UriTemplate("/some/behavior/exception")]
    public class BehaviorExceptionEndpoint : IGet, ITestBehavior
    {
        public const string ExceptionMessage = "This behavior driven endpoint should result in a faulted Task exception.";

        public Status Get()
        {
            throw new Exception(ExceptionMessage);
        }
    }

    public static class TestBehavior
    {
        public static bool Impl(ITestBehavior handler, IContext context, ISimpleContainerScope container)
        {
            return true;
        }
    }

    [RequestBehavior(typeof(TestBehavior), Priority = Priority.Highest)]
    public interface ITestBehavior
    {
    }

    [UriTemplate("/some/behavior/404")]
    public class FourOFourWithBehaviorEndpoint : IGet, ITestBehavior
    {
        public Status Get()
        {
            return Status.NotFound;
        }
    }

    [UriTemplate("/some/long/running")]
    public class SomeLongRunningBehaviorEndpoint : IGet, ITestBehavior
    {
        public Status Get()
        {
            Thread.Sleep(3000);
            return Status.OK;
        }
    }

    [UriTemplate("/some/output/returned")]
    public class SomeOutputReturned : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            this.Output = "Testing 1 2 3";
            return 200;
        }

        public RawHtml Output { get; private set; }
    }
}
