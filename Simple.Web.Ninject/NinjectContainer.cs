using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Ninject
{
    using DependencyInjection;
    using global::Ninject;
    using global::Ninject.Modules;

    public class NinjectContainer : ISimpleContainer
    {
        private readonly global::Ninject.IKernel _kernel;

        internal NinjectContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }
    }

    public abstract class NinjectStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            var module = CreateModules().ToArray();
            var kernel = new StandardKernel(module);
            configuration.Container = new NinjectContainer(kernel);
        }

        internal protected abstract IEnumerable<INinjectModule> CreateModules();
    }

    internal class SimpleNinjectModule : NinjectModule
    {
        private readonly Action<INinjectModule> _load;

        public SimpleNinjectModule(Action<INinjectModule> load)
        {
            _load = load;
        }

        public override void Load()
        {
            _load(this);
        }
    }
}
