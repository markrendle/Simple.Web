using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Ninject.Tests
{
    using CodeGeneration;
    using Xunit;
    using global::Ninject.Modules;

    public class EndpointFactoryBuilderTests
    {
        [Fact]
        public void CreatesInstanceOfType()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var target = new EndpointBuilderFactory(SimpleWeb.Configuration);
            var actualFunc = target.BuildEndpointBuilder(typeof (TestEndpoint));
            var actual = (TestEndpoint)actualFunc(new Dictionary<string, string> { { "TestProperty", "Foo"}});
            Assert.Equal(Status.OK, actual.Get());
            Assert.Equal("Foo", actual.TestProperty);
        }
    }

    public class TestStartup : NinjectStartupBase
    {
        protected override IEnumerable<INinjectModule> CreateModules()
        {
            yield return new TestModule();
        }
    }

    public class TestModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IResult>().To<OkResult>();
        }
    }

    public class TestEndpoint : IGet
    {
        private readonly IResult _result;
        public TestEndpoint(IResult result)
        {
            _result = result;
        }

        public Status Get()
        {
            return _result.Result;
        }

        public string TestProperty { get; set; }
    }

    public interface IResult
    {
        Status Result { get; }
    }

    public class OkResult : IResult
    {
        public Status Result { get { return Status.OK; }}
    }
}
