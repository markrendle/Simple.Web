using StructureMap;

namespace Simple.Web.StructureMap
{
    public abstract class StructureMapStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            ObjectFactory.Configure(Configure);
            configuration.Container = new StructureMapContainer(ObjectFactory.Container);
        }

        internal protected abstract void Configure(ConfigurationExpression cfg);
    }
}