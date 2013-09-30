namespace Simple.Web.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Helpers;

    internal class DefaultSimpleContainer : ISimpleContainer
    {
        public ISimpleContainerScope BeginScope()
        {
            return new DefaultSimpleContainerScope();
        }
    }

    internal class DefaultSimpleContainerScope : ISimpleContainerScope
    {
        public T Get<T>()
        {
            if (typeof(T).IsInterface || typeof(T).IsAbstract)
            {
                T instance;
                if (TryCreateInstance(out instance)) return instance;
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

        private static bool TryCreateInstance<T>(out T instance)
        {
            var implementations = ExportedTypeHelper.FromCurrentAppDomain(IsImplementationOf<T>).ToList();
            if (implementations.Count > 1)
            {
                implementations = ExcludeDefaultImplementations(implementations);
            }
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

        private static List<Type> ExcludeDefaultImplementations(List<Type> implementations)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            implementations = implementations.Where(t => t.Assembly != thisAssembly).ToList();
            return implementations;
        }

        private static bool IsImplementationOf<T>(Type type)
        {
            return (!(type.IsInterface || type.IsAbstract)) && typeof(T).IsAssignableFrom(type);
        }

        public void Dispose()
        {
        }
    }
}