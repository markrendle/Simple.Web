namespace Simple.Web.CodeGeneration.Tests
{
    using System.Collections.Generic;
    using Behaviors;
    using Http;
    using Mocks;
    using Xunit;

    public class HandlerRunnerBuilderTest
    {
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

    class Foo : IGet, IFoo
    {
        public bool Called { get; set; }
        public Status Get()
        {
            return new Status();
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
}
