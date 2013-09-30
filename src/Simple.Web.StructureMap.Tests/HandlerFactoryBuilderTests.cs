﻿namespace Simple.Web.StructureMap.Tests
{
    using System;
    using System.Collections.Generic;

    using global::StructureMap;
    using global::StructureMap.Configuration.DSL;

    using Simple.Web.CodeGeneration;

    using Xunit;

    public class HandlerFactoryBuilderTests
    {
        [Fact]
        public void CreatesInstanceOfType()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var target = new HandlerBuilderFactory(SimpleWeb.Configuration);
            var actualFunc = target.BuildHandlerBuilder(typeof(TestHandler));
            var actual = (TestHandler)actualFunc(new Dictionary<string, string> { { "TestProperty", "Foo" } }).Handler;
            Assert.Equal(Status.OK, actual.Get());
            Assert.Equal("Foo", actual.TestProperty);
        }

        [Fact]
        public void DisposesInstances()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var target = new HandlerBuilderFactory(SimpleWeb.Configuration);
            var actualFunc = target.BuildHandlerBuilder(typeof(TestHandler));

            TestHandler handler;
            using (var scopedHandler = actualFunc(new Dictionary<string, string>()))
            {
                handler = (TestHandler)scopedHandler.Handler;
                Assert.Equal(false, handler.IsDisposed);
            }
            Assert.Equal(true, handler.IsDisposed);
        }
    }

    public class TestStartup : StructureMapStartupBase
    {
        protected internal override void Configure(ConfigurationExpression cfg)
        {
            cfg.Scan(x =>
                     {
                         x.TheCallingAssembly();
                         x.LookForRegistries();
                     });
        }
    }

    public class TestRegistry : Registry
    {
        public TestRegistry()
        {
            For<IResult>().Use<OkResult>();
        }
    }

    public class TestHandler : IGet, IDisposable
    {
        private readonly IResult _result;

        public TestHandler(IResult result)
        {
            _result = result;
        }

        public bool IsDisposed { get; set; }

        public string TestProperty { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public Status Get()
        {
            return _result.Result;
        }
    }

    public interface IResult
    {
        Status Result { get; }
    }

    public class OkResult : IResult
    {
        public Status Result
        {
            get { return Status.OK; }
        }
    }
}