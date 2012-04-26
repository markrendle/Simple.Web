using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    static class SetRequestCookies
    {
        internal static void Impl(IReadCookies setCookies, IContext context)
        {
            setCookies.RequestCookies = context.Request.Cookies;
        }
    }
    
    static class SetResponseCookies
    {
        internal static void Impl(ISetCookies setCookies, IContext context)
        {
            setCookies.ResponseCookies = context.Request.Cookies;
        }
    }
}
