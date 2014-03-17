using System;
using Ninject.Activation.Blocks;

namespace Simple.Web.Ninject
{
    using DependencyInjection;
    using global::Ninject;

    public class NinjectContainer : ISimpleContainer
    {
        private readonly IKernel _kernel;

        internal NinjectContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ISimpleContainerScope BeginScope()
        {
            return new NinjectContainerScope(_kernel.BeginBlock());
        }
    }

    public class NinjectContainerScope: ISimpleContainerScope
    {
        private readonly IActivationBlock _block;

        internal NinjectContainerScope(IActivationBlock block)
        {
            _block = block;
        }

        public T Get<T>()
        {
            return _block.TryGet<T>();
        }

        public void Update<T>(T instance)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _block.Dispose();
        }
    }
}
