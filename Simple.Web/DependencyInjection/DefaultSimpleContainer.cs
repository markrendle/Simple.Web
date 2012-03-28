namespace Simple.Web.DependencyInjection
{
    using System;

    internal class DefaultSimpleContainer : ISimpleContainer
    {
        public T Get<T>()
        {
            try
            {
                return Activator.CreateInstance<T>();
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException("No IoC Container found. Install a Simple.Web IoC container such as Simple.Web.Ninject.");
            }
        }
    }
}