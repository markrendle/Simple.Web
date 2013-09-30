namespace Simple.Web
{
    using System;

    using Simple.Web.Http;

    public interface IExceptionHandler
    {
        Status Handle(Exception exception, IContext context);
    }
}