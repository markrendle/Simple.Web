namespace Simple.Web
{
    /// <summary>
    /// Implement this interface to run code at application startup, before the first request is run.
    /// For example, use a startup task to configure static files and folders, or an IoC container.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Runs the startup task. This method is called by the framework.
        /// </summary>
        /// <param name="configuration">The active Simple.Web configuration.</param>
        /// <param name="environment">The active Simple.Web environment.</param>
        void Run(IConfiguration configuration, IWebEnvironment environment);
    }
}