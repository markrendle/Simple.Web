using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Tests
{
    using Xunit;

    public class EndpointInfoTests
    {
        [Fact]
        public void GetsCorrectInputTypeForGeneric()
        {
            var target = new EndpointInfo(typeof (TestGenericInputEndpoint), "GET");
            Assert.Equal(typeof(EndpointInfoTests), target.InputType);
        }

        [Fact]
        public void GetsNullForNotInputEndpoint()
        {
            var target = new EndpointInfo(typeof(TestEndpoint), "GET");
            Assert.Null(target.InputType);
        }

        [Fact]
        public void GetsCorrectOutputTypeForGeneric()
        {
            var target = new EndpointInfo(typeof (TestGenericOutputEndpoint), "GET");
            Assert.Equal(typeof(EndpointInfoTests), target.OutputType);
        }

        [Fact]
        public void GetsNullForNotOutputEndpoint()
        {
            var target = new EndpointInfo(typeof(TestEndpoint), "GET");
            Assert.Null(target.OutputType);
        }

        class TestGenericInputEndpoint : IInput<EndpointInfoTests>
        {
            public Status Run()
            {
                throw new NotImplementedException();
            }

            public EndpointInfoTests Input
            {
                set { throw new NotImplementedException(); }
            }

            public Type InputType
            {
                get { throw new NotImplementedException(); }
            }
        }

        class TestInputEndpoint : IPost
        {
            public Status Run()
            {
                throw new NotImplementedException();
            }

            public object Input
            {
                set { throw new NotImplementedException(); }
            }

            public Type InputType
            {
                get { throw new NotImplementedException(); }
            }

            public Status Post()
            {
                throw new NotImplementedException();
            }
        }

        class TestGenericOutputEndpoint : IOutput<EndpointInfoTests>
        {
            public Status Run()
            {
                throw new NotImplementedException();
            }

            public EndpointInfoTests Output
            {
                get { throw new NotImplementedException(); }
            }

            public Type OutputType
            {
                get { throw new NotImplementedException(); }
            }
        }

        class TestOutputEndpoint : IGet
        {
            public Status Run()
            {
                throw new NotImplementedException();
            }

            public object Output
            {
                get { throw new NotImplementedException(); }
            }

            public Type OutputType
            {
                get { throw new NotImplementedException(); }
            }

            public Status Get()
            {
                throw new NotImplementedException();
            }
        }

        class TestEndpoint : IGet
        {
            public Status Get()
            {
                throw new NotImplementedException();
            }
        }
    }

}
