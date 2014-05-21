namespace Sandbox.StartupTasks
{
    using Simple.Web;
    using Simple.Web.Authentication;
    using Simple.Web.JsonNet;
    using Simple.Web.MediaTypeHandling;

    public class AppStart : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            SimpleWeb.Configuration.AuthenticationProvider = new DefaultAuthenticationProvider();
            SimpleWeb.Configuration.ExceptionHandler = new ExceptionHandler();
            SimpleWeb.Configuration.DefaultMediaTypeHandler = new JsonMediaTypeHandler();
            MediaTypeHandlers.For("application/json").Use<JsonMediaTypeHandlerWithDeepLinks>();
        }
    }
}