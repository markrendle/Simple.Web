namespace Simple.Web.Windsor
{
    using System.Collections.Generic;
    using System.Linq;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    public abstract class WindsorStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            var windsorInstallers = GetInstallers().ToArray();
            if (windsorInstallers.Length == 0)
            {
                return;
            }

            var container = new WindsorContainer();
            container.Install(windsorInstallers);

            configuration.Container = new WindsorSimpleContainer(container);
        }

        protected abstract IEnumerable<IWindsorInstaller> GetInstallers();
    }
}