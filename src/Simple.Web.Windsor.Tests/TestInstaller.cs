namespace Simple.Web.Windsor.Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class TestInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IResult>().ImplementedBy<OkResult>().LifestyleScoped());
            container.Register(Component.For<TestHandler>().ImplementedBy<TestHandler>().LifestyleScoped());
        }
    }
}