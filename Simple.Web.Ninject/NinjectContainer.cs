namespace Simple.Web.Ninject
{
    using System.Collections.Generic;
    using System.Linq;
    using DependencyInjection;
    using global::Ninject;
    using global::Ninject.Modules;

    public class NinjectContainer : ISimpleContainer
    {
        private readonly IKernel _kernel;

        internal NinjectContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T Get<T>()
        {
            return _kernel.TryGet<T>();
        }
    }

    public abstract class NinjectStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            var module = CreateModules().ToArray();
            if (module.Length == 0) return;
            var kernel = new StandardKernel(module);
            configuration.Container = new NinjectContainer(kernel);
        }

        internal protected abstract IEnumerable<INinjectModule> CreateModules();
    }
}
