namespace Sandbox.StartupTasks
{
    using Simple.Web;

    public class SetPublicFolders : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            configuration.PublicFolders.Add("/Scripts");
            configuration.PublicFileMappings.Add("/about", "/about.html");
            configuration.AuthenticatedFileMappings.Add("/secure", "/secure.html");
            configuration.AuthenticatedFileMappings.Add("/secure.html", "/secure.html");
        }
    }
}