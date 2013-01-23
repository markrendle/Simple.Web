namespace Simple.Web.Autofac
{
    using DependencyInjection;
    using global::Autofac;

    public class AutofacContainer : ISimpleContainer
    {
        private readonly IContainer _container;

        internal AutofacContainer(IContainer container)
        {
            _container = container;
        }

        public ISimpleContainerScope BeginScope()
        {
            return new AutofacContainerScope(_container.BeginLifetimeScope());
        }
    }

    public class AutofacContainerScope : ISimpleContainerScope
    {
        private readonly ILifetimeScope _scope;

        internal AutofacContainerScope(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public T Get<T>()
        {
            T instance;

            return _scope.TryResolve<T>(out instance) ? instance : default(T);            
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
