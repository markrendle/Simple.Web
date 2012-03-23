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

    class StaticEndpoint : IGet, IOutput<object>
    {
        public object Output
        {
            get { throw new System.NotImplementedException(); }
        }

        public Status Get()
        {
            throw new System.NotImplementedException();
        }
    }

    [UriTemplate("/Tests/{Id}")]
    class DynamicEndpoint : IGet, IOutput<object>
    {
        public int Id { get; set; }

        public object Output
        {
            get { throw new System.NotImplementedException(); }
        }

        public Status Get()
        {
            throw new System.NotImplementedException();
        }
    }
}