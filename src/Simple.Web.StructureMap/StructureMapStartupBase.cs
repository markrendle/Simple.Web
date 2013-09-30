namespace Simple.Web.StructureMap
{
    using global::StructureMap;

    public abstract class StructureMapStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            ObjectFactory.Configure(Configure);
            configuration.Container = new StructureMapContainer(ObjectFactory.Container);
        }

        protected internal abstract void Configure(ConfigurationExpression cfg);
    }
}