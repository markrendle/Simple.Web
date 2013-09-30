namespace Simple.Web.Windsor.Tests
{
    using System.Collections.Generic;

    using Castle.MicroKernel.Registration;

    public class TestStartup : WindsorStartupBase
    {
        protected override IEnumerable<IWindsorInstaller> GetInstallers()
        {
            return new List<IWindsorInstaller> { new TestInstaller() };
        }
    }
}