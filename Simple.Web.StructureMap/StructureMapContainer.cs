using System;
using Simple.Web.DependencyInjection;
using StructureMap;

namespace Simple.Web.StructureMap
{
    public class StructureMapContainer : ISimpleContainer
    {
        private readonly IContainer _container;

        internal StructureMapContainer(IContainer container)
        {
            _container = container;
        }

        public ISimpleContainerScope BeginScope()
        {
            return new StructureMapContainerScope(_container.GetNestedContainer());
        }
    }

    public class StructureMapContainerScope: ISimpleContainerScope
    {
        private readonly IContainer _container;

        internal StructureMapContainerScope(IContainer container)
        {
            _container = container;
        }

        public T Get<T>()
        {
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                return _container.TryGetInstance<T>();
            }
            else
            {
                return _container.GetInstance<T>();
            }
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
