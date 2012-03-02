namespace Simple.Web.Tests
{
    using System.Collections.Generic;
    using Xunit;

    public class EndpointFactoryTests
    {
        [Fact]
        public void ShouldCreateEmptyEndpoint()
        {
            var actual = new EndpointFactory().GetEndpoint(typeof (StaticEndpoint), new Dictionary<string, string>());
            Assert.NotNull(actual);
        }

        [Fact]
        public void ShouldCreateEndpointAndPopulateVariable()
        {
            var actual = new EndpointFactory().GetEndpoint(typeof(DynamicEndpoint), new Dictionary<string, string> { { "Id", "42"}}) as DynamicEndpoint;
            Assert.NotNull(actual);
            Assert.Equal(42, actual.Id);
        }
    }

    class StaticEndpoint : IEndpoint
    {
        public string UriTemplate
        {
            get { return "/"; }
        }

        public object Run()
        {
            throw new System.NotImplementedException();
        }
    }

    class DynamicEndpoint : IEndpoint
    {
        public string UriTemplate
        {
            get { return "/Tests/{Id}"; }
        }

        public int Id { get; set; }

        public object Run()
        {
            throw new System.NotImplementedException();
        }
    }
}