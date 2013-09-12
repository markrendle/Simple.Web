namespace Simple.Web.OwinSupport
{
    using System;

    using Owin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public static class OwinHelpers
    {
        public static void UseSimpleWeb(this IAppBuilder app)
        {
            app.Use(new Func<AppFunc, AppFunc>(ignoreNextApp => (AppFunc)Application.Run));
        }
    }
}