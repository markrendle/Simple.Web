using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultSimpleContainer.Test
{
    using Simple.Web;
    using Simple.Web.Authentication;
    using Simple.Web.Http;
    using Xunit;

    public class AuthenticationProviderTest
    {
        [Fact]
        public void DefaultSimpleContainerChoosesNonDefaultAuthenticationProviderWhenPresent()
        {
            var config = new Simple.Web.Configuration();
            Assert.IsType<CustomAuthenticationProvider>(config.AuthenticationProvider);
        }
    }

    public class CustomAuthenticationProvider : IAuthenticationProvider
    {
        public IUser GetLoggedInUser(IContext context)
        {
            throw new NotImplementedException();
        }

        public void SetLoggedInUser(IContext context, IUser user)
        {
            throw new NotImplementedException();
        }
    }
}
