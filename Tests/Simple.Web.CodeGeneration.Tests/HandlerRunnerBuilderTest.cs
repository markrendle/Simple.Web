namespace Simple.Web.CodeGeneration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Behaviors;
    using Helpers;
    using Http;
    using MediaTypeHandling;
    using Mocks;
    using Xunit;

    public class HandlerRunnerBuilderTest
    {
        static readonly IMediaTypeHandler JsonMediaTypeHandler = new TestJsonMediaTypeHandler();
        private static readonly byte[] TestJson = Encoding.UTF8.GetBytes("{\"Called\": true, \"Test\":\"Pass\"}\r\n");

        [Fact]
        public void CallsFooImplementation()
        {
            var target = new HandlerRunnerBuilder(typeof (Foo), "GET").BuildRunner();
            var context = new Mocks.MockContext();
            context.Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } } } };
            var foo = new Foo();
            target(foo, context);
            Assert.True(foo.Called);
        }

        [Fact]
        public void RedirectFooRedirects()
        {
            var target = new HandlerRunnerBuilder(typeof (RedirectFoo), "GET").BuildRunner();
            var context = new Mocks.MockContext();
            context.Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } } } };
            context.Response = new MockResponse();
            var foo = new RedirectFoo();
            target(foo, context);
            Assert.True(context.Response.Headers.ContainsKey("Location"));
        }

        [Fact]
        public void CallsPostWithParameter()
        {
            var target = new HandlerRunnerBuilder(typeof (PostFoo), "POST").BuildRunner();
            var context = new Mocks.MockContext();
            context.Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } }, {HeaderKeys.ContentType, new[] { "application/json" }} },
                InputStream = new MemoryStream(TestJson)};
            var postFoo = new PostFoo();
            target(postFoo, context);
            Assert.True(postFoo.Called);
            Assert.Equal("Pass", postFoo.Test);
        }

        [Fact]
        public void CallsPutWithParameter()
        {
            var target = new HandlerRunnerBuilder(typeof (PutFoo), "PUT").BuildRunner();
            var context = new Mocks.MockContext();
            context.Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } }, {HeaderKeys.ContentType, new[] { "application/json" }} },
                InputStream = new MemoryStream(TestJson)};
            var postFoo = new PutFoo();
            target(postFoo, context);
            Assert.True(postFoo.Called);
            Assert.Equal("Pass", postFoo.Test);
        }
        
        [Fact]
        public void CallsPatchWithParameter()
        {
            var target = new HandlerRunnerBuilder(typeof (PatchFoo), "PATCH").BuildRunner();
            var context = new Mocks.MockContext();
            context.Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } }, {HeaderKeys.ContentType, new[] { "application/json" }} },
                InputStream = new MemoryStream(TestJson)};
            var postFoo = new PatchFoo();
            target(postFoo, context);
            Assert.True(postFoo.Called);
            Assert.Equal("Pass", postFoo.Test);
        }
        
        [Fact]
        public void BarStopsFoo()
        {
            var target = new HandlerRunnerBuilder(typeof (Bar), "GET").BuildRunner();
            var context = new Mocks.MockContext();
            context.Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } } } };
            var bar = new Bar();
            target(bar, context);
            Assert.True(((IBar)bar).Called);
            Assert.False(((IFoo)bar).Called);
        }
    }

    class Bar : IGet, IFoo, IBar
    {
        bool IFoo.Called { get; set; }
        bool IBar.Called { get; set; }

        public Status Get()
        {
            return new Status();
        }
    }

    public class PostFoo : IPost<FooModel>
    {
        public string Test;
        public bool Called;
        public Status Post(FooModel input)
        {
            Called = input.Called;
            Test = input.Test;
            return 200;
        }
    }

    public class PutFoo : IPut<FooModel>
    {
        public string Test;
        public bool Called;
        public Status Put(FooModel input)
        {
            Called = input.Called;
            Test = input.Test;
            return 200;
        }
    }

    public class PatchFoo : IPatch<FooModel>
    {
        public string Test;
        public bool Called;
        public Status Patch(FooModel input)
        {
            Called = input.Called;
            Test = input.Test;
            return 200;
        }
    }

    class Foo : IGet, IFoo
    {
        public bool Called { get; set; }
        public Status Get()
        {
            return new Status();
        }
    }

    class RedirectFoo : IGet
    {
        public Status Get()
        {
            return Status.SeeOther("http://pass");
        }
    }

    [RequestBehavior(typeof(FooImpl))]
    public interface IFoo
    {
        bool Called { get; set; }
    }

    public static class FooImpl
    {
        public static bool Called;
        public static void Impl(IFoo foo, IContext context)
        {
            foo.Called = true;
        }
    }
    
    [RequestBehavior(typeof(BarImpl), Priority = Priority.Highest)]
    public interface IBar
    {
        bool Called { get; set; }
    }

    public static class BarImpl
    {
        public static bool Called;
        public static bool Impl(IBar bar, IContext context)
        {
            bar.Called = true;
            return false;
        }
    }

    public class FooModel
    {
        public string Test { get; set; }
        public bool Called { get; set; }
    }

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class TestJsonMediaTypeHandler : IMediaTypeHandler
    {
        public object Read(Stream inputStream, Type inputType)
        {
            return new FooModel()
            {
                Test = "Pass",
                Called = true
            };
        }

        public Task Write(IContent content, Stream outputStream)
        {
            return TaskHelper.Completed();
        }
    }
}
