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
        	return IsConcrete(typeof(T)) ? _container.GetInstance<T>() : _container.TryGetInstance<T>();
        }

        public object Get(Type objectType)
        {
            return IsConcrete(objectType) ? _container.GetInstance(objectType) : _container.TryGetInstance(objectType);
        }

        private static bool IsConcrete(Type type)
    	{
            return !(type.IsAbstract || type.IsInterface);
    	}

    	public void Dispose()
        {
            _container.Dispose();
        }
    }
}
