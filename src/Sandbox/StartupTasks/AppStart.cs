namespace Sandbox.StartupTasks
{
    using Simple.Web;
    using Simple.Web.Authentication;
    using Simple.Web.JsonNet;

    public class AppStart : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            SimpleWeb.Configuration.AuthenticationProvider = new DefaultAuthenticationProvider();
            SimpleWeb.Configuration.DefaultMediaTypeHandler = new JsonMediaTypeHandler();
        }
    }
}