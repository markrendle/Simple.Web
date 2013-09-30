namespace Simple.Web.Tests
{
    using System;

    using Simple.Web.Behaviors;
    using Simple.Web.Hosting;

    using Xunit;

    public class HandlerInfoTests
    {
        [Fact]
        public void GetsCorrectInputTypeForGeneric()
        {
            var target = new HandlerInfo(typeof(TestGenericInputHandler), "GET");
            Assert.Equal(typeof(HandlerInfoTests), target.InputType);
        }

        [Fact]
        public void GetsCorrectOutputTypeForGeneric()
        {
            var target = new HandlerInfo(typeof(TestGenericOutputHandler), "GET");
            Assert.Equal(typeof(HandlerInfoTests), target.OutputType);
        }

        [Fact]
        public void GetsNullForNotInputHandler()
        {
            var target = new HandlerInfo(typeof(TestHandler), "GET");
            Assert.Null(target.InputType);
        }

        [Fact]
        public void GetsNullForNotOutputHandler()
        {
            var target = new HandlerInfo(typeof(TestHandler), "GET");
            Assert.Null(target.OutputType);
        }

        private class TestGenericInputHandler : IInput<HandlerInfoTests>
        {
            public HandlerInfoTests Input
            {
                set { throw new NotImplementedException(); }
            }

            public Type InputType
            {
                get { throw new NotImplementedException(); }
            }

            public Status Run()
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenericOutputHandler : IOutput<HandlerInfoTests>
        {
            public HandlerInfoTests Output
            {
                get { throw new NotImplementedException(); }
            }

            public Type OutputType
            {
                get { throw new NotImplementedException(); }
            }

            public Status Run()
            {
                throw new NotImplementedException();
            }
        }

        private class TestHandler : IGet
        {
            public Status Get()
            {
                throw new NotImplementedException();
            }
        }

        private class TestInputHandler : IPost
        {
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

            public Status Run()
            {
                throw new NotImplementedException();
            }
        }

        private class TestOutputHandler : IGet
        {
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

            public Status Run()
            {
                throw new NotImplementedException();
            }
        }
    }
}