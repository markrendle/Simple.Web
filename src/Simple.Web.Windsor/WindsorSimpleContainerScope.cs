using System;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Simple.Web.DependencyInjection;

namespace Simple.Web.Windsor
{
    public class WindsorSimpleContainerScope: ISimpleContainerScope
    {
        readonly IWindsorContainer _container;
        readonly IDisposable _scope;
       
        internal WindsorSimpleContainerScope(IWindsorContainer container)
        {
            _container = container;
            _scope = _container.BeginScope();
        }

        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}