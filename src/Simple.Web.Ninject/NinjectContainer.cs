namespace Simple.Web.Ninject
{
    using System;

    using global::Ninject;
    using global::Ninject.Activation.Blocks;

    using Simple.Web.DependencyInjection;

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

    public class NinjectContainerScope : ISimpleContainerScope
    {
        private readonly IActivationBlock _block;

        internal NinjectContainerScope(IActivationBlock block)
        {
            _block = block;
        }

        public void Dispose()
        {
            _block.Dispose();
        }

        public T Get<T>()
        {
            return _block.TryGet<T>();
        }

        public object Get(Type objectType)
        {
            return _block.TryGet(objectType);
        }
    }
}