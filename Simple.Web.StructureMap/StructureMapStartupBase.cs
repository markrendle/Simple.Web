using StructureMap;

namespace Simple.Web.StructureMap
{
    public abstract class StructureMapStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            var container = new Container(Configure);
            configuration.Container = new StructureMapContainer(container);
        }

        internal protected abstract void Configure(ConfigurationExpression cfg);
    }
}