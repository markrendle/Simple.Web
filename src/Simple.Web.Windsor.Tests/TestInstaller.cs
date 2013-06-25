using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Simple.Web.Windsor.Tests
{
    public class TestInstaller : IWindsorInstaller
    {
        public TestInstaller() {}

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IResult>().ImplementedBy<OkResult>().LifestyleScoped());
            container.Register(Component.For<TestHandler>().ImplementedBy<TestHandler>().LifestyleScoped());
        }
    }
}