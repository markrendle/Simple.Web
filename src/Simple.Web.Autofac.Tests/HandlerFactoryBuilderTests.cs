using System;
using System.Collections.Generic;
using Autofac;

namespace Simple.Web.Autofac.Tests
{
    using System.Reflection;
    using CodeGeneration;
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

            var actual = (TestHandler)actualFunc(
                new Dictionary<string, string[]>
                    {
                        { "TestProperty", new[] { "Foo" } }
                    }).Handler;

            Assert.Equal(Status.OK, actual.Get());
            Assert.Equal("Foo", actual.TestProperty);
        }

        [Fact]
        public void CreatesInstanceOfGenericType()
        {
            var startup = new TestStartup();

            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);

            var target = new HandlerBuilderFactory(SimpleWeb.Configuration);

            var actualFunc = target.BuildHandlerBuilder(typeof(GenericTestHandler));

            var actual = (GenericTestHandler)actualFunc(
                new Dictionary<string, string[]>
                    {
                        { "TestProperty", null }
                    }).Handler;

            var status = actual.Patch(new GenericArgument() {Name = "Foo"});

            Assert.Equal(Status.Created, status);
            Assert.Equal("Foo", actual.TestProperty.Name);
        }


        [Fact]
        public void DisposesInstances()
        {
            var startup = new TestStartup();

            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);

            var target = new HandlerBuilderFactory(SimpleWeb.Configuration);

            var actualFunc = target.BuildHandlerBuilder(typeof(TestHandler));

            TestHandler handler;
            using (var scopedHandler = actualFunc(new Dictionary<string, string[]>()))
            {
                handler = (TestHandler)scopedHandler.Handler;
                Assert.Equal(false, handler.IsDisposed);
            }
            Assert.Equal(true, handler.IsDisposed);
        }
    }

    public class TestStartup : AutofacStartupBase
    {
        protected internal override IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
                        
            builder.RegisterHandlersInAssembly(Assembly.GetExecutingAssembly());


            builder.Register(c => Status.Created)
                .AsSelf();
            
            builder.RegisterType<OkResult>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return builder.Build();
        }
    }

    public class TestHandler : IGet, IDisposable
    {
        private readonly IResult _result;
        public bool IsDisposed { get; set; }

        public TestHandler(IResult result)
        {
            _result = result;
        }

        public Status Get()
        {
            return _result.Result;
        }

        public string TestProperty { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public class GenericTestHandler : IPatch<GenericArgument>
    {
        private readonly Status _status;
        public GenericArgument TestProperty { get; set; }

        public GenericTestHandler(Status status)
        {
            _status = status;
        }

        public Status Patch(GenericArgument input)
        {
            TestProperty = input;
            return _status;
        }
    }


    public interface IResult
    {
        Status Result { get; }
    }

    public class OkResult : IResult
    {
        public Status Result { get { return Status.OK; }}
    }

    public class GenericArgument
    {
        public string Name { set;  get; }
        
    }
}
