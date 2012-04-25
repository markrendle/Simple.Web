using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    static class SetCookies
    {
        internal static void Impl(ISetCookies setCookies, IContext context)
        {
            foreach (var cookie in setCookies.CookiesToSet)
            {
                context.Response.SetCookie(cookie);
            }
        }
    }
}
