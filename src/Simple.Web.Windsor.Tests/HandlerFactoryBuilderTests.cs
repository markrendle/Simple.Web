namespace Simple.Web.Windsor.Tests
{
    using System.Collections.Generic;

    using Simple.Web.CodeGeneration;

    using Xunit;

    public class HandlerFactoryBuilderTests
    {
        [IgnoreOnMonoFact]
        public void CreatesInstanceOfType()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var builderFactory = new HandlerBuilderFactory(SimpleWeb.Configuration);
            var handlerFactory = builderFactory.BuildHandlerBuilder(typeof(TestHandler));
            var handler = (TestHandler)handlerFactory(new Dictionary<string, string> { { "TestProperty", "Foo" } }).Handler;
            Assert.Equal(Status.OK, handler.Get());
            Assert.Equal("Foo", handler.TestProperty);
        }

        [IgnoreOnMonoFact]
        public void DisposesInstances()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var builderFactory = new HandlerBuilderFactory(SimpleWeb.Configuration);
            var handlerFactory = builderFactory.BuildHandlerBuilder(typeof(TestHandler));

            TestHandler handler;
            using (var scopedHandler = handlerFactory(new Dictionary<string, string>()))
            {
                handler = (TestHandler)scopedHandler.Handler;
                Assert.Equal(false, handler.IsDisposed);
            }
            Assert.Equal(true, handler.IsDisposed);
        }
    }
}