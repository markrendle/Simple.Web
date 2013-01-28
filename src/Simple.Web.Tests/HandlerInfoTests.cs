using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Tests
{
    using Behaviors;
    using Hosting;
    using Xunit;

    public class HandlerInfoTests
    {
        [Fact]
        public void GetsCorrectInputTypeForGeneric()
        {
            var target = new HandlerInfo(typeof (TestGenericInputHandler), "GET");
            Assert.Equal(typeof(HandlerInfoTests), target.InputType);
        }

        [Fact]
        public void GetsNullForNotInputHandler()
        {
            var target = new HandlerInfo(typeof(TestHandler), "GET");
            Assert.Null(target.InputType);
        }

        [Fact]
        public void GetsCorrectOutputTypeForGeneric()
        {
            var target = new HandlerInfo(typeof (TestGenericOutputHandler), "GET");
            Assert.Equal(typeof(HandlerInfoTests), target.OutputType);
        }

        [Fact]
        public void GetsNullForNotOutputHandler()
        {
            var target = new HandlerInfo(typeof(TestHandler), "GET");
            Assert.Null(target.OutputType);
        }

        class TestGenericInputHandler : IInput<HandlerInfoTests>
        {
            public Status Run()
            {
                throw new NotImplementedException();
            }

            public HandlerInfoTests Input
            {
                set { throw new NotImplementedException(); }
            }

            public Type InputType
            {
                get { throw new NotImplementedException(); }
            }
        }

        class TestInputHandler : IPost
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

        class TestGenericOutputHandler : IOutput<HandlerInfoTests>
        {
            public Status Run()
            {
                throw new NotImplementedException();
            }

            public HandlerInfoTests Output
            {
                get { throw new NotImplementedException(); }
            }

            public Type OutputType
            {
                get { throw new NotImplementedException(); }
            }
        }

        class TestOutputHandler : IGet
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

        class TestHandler : IGet
        {
            public Status Get()
            {
                throw new NotImplementedException();
            }
        }
    }

}
