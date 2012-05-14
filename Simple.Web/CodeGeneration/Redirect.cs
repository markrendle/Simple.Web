using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    static class Redirect
    {
        public static bool Impl(IMayRedirect handler, Status status, IContext context)
        {
            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                context.Response.Headers.Set("Location", handler.Location);
                return true;
            }
            return false;
        }
    }
}
