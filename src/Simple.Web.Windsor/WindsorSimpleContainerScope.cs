using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Simple.Web.DependencyInjection;

namespace Simple.Web.Windsor
{
    public class WindsorSimpleContainerScope: ISimpleContainerScope
    {
        private readonly IWindsorContainer _parentContainer;
        readonly IWindsorContainer _container;
       
        internal WindsorSimpleContainerScope(IWindsorContainer parentContainer)
        {
            _parentContainer = parentContainer;
            _container = new WindsorContainer();
            _parentContainer.AddChildContainer(_container);
        }

        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        public void Dispose()
        {
            _parentContainer.RemoveChildContainer(_container);
            _container.Dispose();
        }
    }
}