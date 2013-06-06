using Castle.Windsor;
using Simple.Web.DependencyInjection;

namespace Simple.Web.Windsor
{
    public class WindsorSimpleContainer : ISimpleContainer
    {
        readonly IWindsorContainer _container;

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
