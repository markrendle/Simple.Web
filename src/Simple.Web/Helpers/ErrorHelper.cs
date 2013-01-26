﻿namespace Simple.Web.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using Http;

    /// <summary>
    /// Used for writing error messages to a response.
    /// </summary>
    public class ErrorHelper
    {
        private readonly IContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHelper"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ErrorHelper(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Writes the error to the Response.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void WriteError(Exception exception)
        {
            Trace.TraceError(exception.Message);
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                _context.Response.Status = string.Format("{0} {1}", httpException.ErrorCode, httpException.Message);
            }
            else
            {
                _context.Response.Status = "500 Internal server error.";
            }
            _context.Response.SetContentType("text/html");
            _context.Response.Write(exception.ToString());
        }
    }
}