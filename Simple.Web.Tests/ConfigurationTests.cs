using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Tests
{
    using Xunit;

    public class ConfigurationTests
    {
        [Fact]
        public void KnownContentTypeReturnsCorrectHandler()
        {
            
        }
    }

    class TestConfiguration : Configuration
    {
        protected override void Configure()
        {
            AddContentTypeHandler(() => null, "text/text");
        }
    }
}
