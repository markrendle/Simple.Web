namespace Simple.Web.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Web;

    public class ErrorHelper
    {
        private readonly IContext _context;

        public ErrorHelper(IContext context)
        {
            _context = context;
        }

        public void WriteError(Exception exception)
        {
            Trace.TraceError(exception.Message);
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                _context.Response.StatusCode = httpException.ErrorCode;
                _context.Response.StatusDescription = httpException.Message;
            }
            else
            {
                _context.Response.StatusCode = 500;
                _context.Response.StatusDescription = "Internal server error.";
            }
            _context.Response.ContentType = "text/text";
            _context.Response.Write(exception.ToString());
        }
    }
}