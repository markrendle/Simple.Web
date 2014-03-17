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
        bool _disposed;

        internal WindsorSimpleContainerScope(IWindsorContainer container)
        {
            _container = container;
            _scope = _container.BeginScope();
        }

        public T Get<T>()
        {
            EnsureNotDisposed();

            return _container.Kernel.Resolve<T>();
        }

        public void Update<T>(T instance)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _scope.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }


        void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("WindsorDependencyScope");
            }
        }
    }
}