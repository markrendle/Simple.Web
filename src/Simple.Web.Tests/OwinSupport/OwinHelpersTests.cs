using System.Collections.Generic;

namespace Simple.Web.Tests.OwinSupport
{
    using System;
    using Owin;
    using Web.OwinSupport;
    using Xunit;

    using AppFunc = System.Func<IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class OwinHelpersTests
    {
        [Fact]
        public void UseSimpleWebUseCompatibleDelegate()
        {
            var mockAppBuilder = new MockAppBuilder();

            mockAppBuilder.UseSimpleWeb();

            Assert.IsAssignableFrom<Func<AppFunc, AppFunc>>(mockAppBuilder.AssignedMiddleWare);
        }

        private class MockAppBuilder : IAppBuilder
        {
            public IAppBuilder Use(object middleware, params object[] args)
            {
                AssignedMiddleWare = middleware;
                return this;
            }

            public object AssignedMiddleWare { get; private set; }

            public object Build(Type returnType)
            {
                throw new NotImplementedException();
            }

            public IAppBuilder New()
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, object> Properties { get; private set; }
        }

    }
}