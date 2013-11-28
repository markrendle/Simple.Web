namespace Simple.Web.OwinSupport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Owin;

    public static class OwinHelpers
    {
        public static void UseSimpleWeb(this IAppBuilder app)
        {
            app.Use(new Func<IDictionary<string,object>, Func<IDictionary<string,object>, Task>, Task>(Application.Run));
        }
    }
}