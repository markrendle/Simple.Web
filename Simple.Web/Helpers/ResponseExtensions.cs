using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Helpers
{
    using Http;

    /// <summary>
    /// Extension methods for <see cref="IResponse"/>.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Sets the status code and description from a <see cref="Status"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="status">The status.</param>
        public static void SetStatus(this IResponse response, Status status)
        {
            response.StatusCode = status.Code;
            response.StatusDescription = status.Description;
        }
    }
}
