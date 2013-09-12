using System.Collections.Generic;
using Castle.MicroKernel.Registration;

namespace Simple.Web.Windsor.Tests
{
    public class TestStartup : WindsorStartupBase
    {
        protected override IEnumerable<IWindsorInstaller> GetInstallers()
        {
            return new List<IWindsorInstaller>
            {
                new TestInstaller() 
            };
        }
    }
}