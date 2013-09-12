using System;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Simple.Web.DependencyInjection;

namespace Simple.Web.Windsor
{
    public class WindsorSimpleContainerScope : ISimpleContainerScope
    {
        private readonly IWindsorContainer _container;
        private readonly IDisposable _scope;
        private bool _disposed;

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

        public object Get(Type objectType)
        {
            EnsureNotDisposed();

            return _container.Kernel.Resolve(objectType);
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


        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("WindsorDependencyScope");
            }
        }
    }
}