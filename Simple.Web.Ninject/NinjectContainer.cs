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

        public T Get<T>()
        {
            return _kernel.TryGet<T>();
        }
    }
}
