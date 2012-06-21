namespace SelfHost.Startup {
    using Simple.Web;

    public class AppStart : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            //SimpleWeb.Configuration.AuthenticationProvider = new DefaultAuthenticationProvider();
        }
    }
}
