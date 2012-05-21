using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Helpers
{
    public static class ResponseExtensions
    {
        public static void SetStatus(this IResponse response, Status status)
        {
            response.StatusCode = status.Code;
            response.StatusDescription = status.Description;
        }
    }
}
