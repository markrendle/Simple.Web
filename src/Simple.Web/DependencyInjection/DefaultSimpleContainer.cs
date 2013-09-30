namespace Simple.Web.DependencyInjection
{
    using System;
    using System.Linq;

    using Simple.Web.Helpers;

    internal class DefaultSimpleContainer : ISimpleContainer
    {
        public ISimpleContainerScope BeginScope()
        {
            return new DefaultSimpleContainerScope();
        }
    }

    internal class DefaultSimpleContainerScope : ISimpleContainerScope
    {
        public void Dispose()
        {
        }

        public T Get<T>()
        {
            if (typeof(T).IsInterface || typeof(T).IsAbstract)
            {
                T instance;
                if (TryCreateInstance(out instance))
                {
                    return instance;
                }
                throw new InvalidOperationException("No IoC Container found. Install a Simple.Web IoC container such as Simple.Web.Ninject.");
            }
            try
            {
                return Activator.CreateInstance<T>();
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException("No IoC Container found. Install a Simple.Web IoC container such as Simple.Web.Ninject.");
            }
        }

        public object Get(Type objectType)
        {
            throw new NotImplementedException();
        }

        private static bool IsImplementationOf<T>(Type type)
        {
            return (!(type.IsInterface || type.IsAbstract)) && typeof(T).IsAssignableFrom(type);
        }

        private static bool TryCreateInstance<T>(out T instance)
        {
            var implementations = ExportedTypeHelper.FromCurrentAppDomain(IsImplementationOf<T>).ToList();
            if (implementations.Count == 1)
            {
                if (implementations[0].GetConstructor(new Type[0]) != null)
                {
                    {
                        instance = (T)Activator.CreateInstance(implementations[0]);
                        return true;
                    }
                }
            }
            instance = default(T);
            return false;
        }
    }
}