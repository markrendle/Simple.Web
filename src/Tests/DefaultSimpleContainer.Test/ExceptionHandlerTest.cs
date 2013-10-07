using System;

namespace DefaultSimpleContainer.Test
{
    using Simple.Web;
    using Simple.Web.Http;
    using Xunit;

    public class ExceptionHandlerTest
    {
        [Fact]
        public void DefaultSimpleContainerChoosesExceptionHandlerWhenPresent()
        {
            var config = new Simple.Web.Configuration();
            Assert.IsType<CustomExceptionHandler>(config.ExceptionHandler);
        }

        public class CustomExceptionHandler : IExceptionHandler
        {
            public Status Handle(Exception exception, IContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}