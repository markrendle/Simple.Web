namespace Simple.Web.Windsor
{
    using Castle.Windsor;

    using Simple.Web.DependencyInjection;

    public class WindsorSimpleContainer : ISimpleContainer
    {
        private readonly IWindsorContainer _container;

        internal WindsorSimpleContainer(IWindsorContainer container)
        {
            _container = container;
        }

        public ISimpleContainerScope BeginScope()
        {
            return new WindsorSimpleContainerScope(_container);
        }
    }
}