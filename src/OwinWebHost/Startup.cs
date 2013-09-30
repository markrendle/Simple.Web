using Microsoft.Owin;

using OwinWebHost;

[assembly: OwinStartup(typeof(Startup))]

namespace OwinWebHost
{
    using Owin;

    using Simple.Web.OwinSupport;

    public class Startup : OwinStartupBase
    {
    }

    public class StartupWithCustomMiddleware : OwinStartupBase
    {
        public StartupWithCustomMiddleware()
            : base(builder =>
                   {
                       builder.UseErrorPage();
                       builder.UseWelcomePage("/owin");
                       builder.UseSimpleWeb();
                   })
        {
        }
    }
}