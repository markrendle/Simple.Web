[assembly: Microsoft.Owin.OwinStartup(typeof(OwinLibraryHost.Startup))]

namespace OwinLibraryHost
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