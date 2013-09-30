namespace Sandbox
{
    using System;

    using Simple.Web;
    using Simple.Web.Http;

    public class ExceptionHandler : IExceptionHandler
    {
        public Status Handle(Exception exception, IContext context)
        {
            if (exception is NotImplementedException)
            {
                return new Status(410, "Not implemented");
            }

            return new Status(500, "Oh bugger");
        }
    }
}