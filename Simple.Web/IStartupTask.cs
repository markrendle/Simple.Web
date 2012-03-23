namespace Simple.Web
{
    public interface IStartupTask
    {
        void Run(IConfiguration configuration, IWebEnvironment environment);
    }
}