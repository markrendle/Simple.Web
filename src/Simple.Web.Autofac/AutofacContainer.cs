﻿namespace Simple.Web.Autofac
{
    using System;

    using global::Autofac;

    using Simple.Web.DependencyInjection;

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

        public void Dispose()
        {
            _scope.Dispose();
        }

        public T Get<T>()
        {
            T instance;
            _scope.TryResolve(out instance);
            return instance;
        }

        public object Get(Type objectType)
        {
            object instance;
            _scope.TryResolve(objectType, out instance);
            return instance;
        }
    }
}