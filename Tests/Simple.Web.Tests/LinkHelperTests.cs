namespace Simple.Web.Tests
{
    using System.Linq;
    using Helpers;
    using Links;
    using Xunit;

    public class LinkHelperTests
    {
        [Fact]
        public void BuildsLinkUsingCustomUriTemplate()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetLinksForModel(customer).Single(l => l.GetHandlerType() == typeof(CustomerOrdersHandler));
            Assert.NotNull(link);
            Assert.Equal("/customers/42/orders", link.Href);
            Assert.Equal("customer.orders", link.Rel);
            Assert.Equal("application/vnd.list.order", link.Type);
        }
        
        [Fact]
        public void BuildsLinkUsingDefaultUriTemplate()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetLinksForModel(customer).Single(l => l.GetHandlerType() == typeof(CustomerLocationHandler));
            Assert.NotNull(link);
            Assert.Equal("/customers/42/location", link.Href);
            Assert.Equal("customer.location", link.Rel);
            Assert.Equal("application/vnd.location", link.Type);
        }
        
        [Fact]
        public void BuildsCanonicalLink()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetCanonicalLinkForModel(customer);
            Assert.NotNull(link);
            Assert.Equal("/customer/42", link.Href);
            Assert.Equal("self", link.Rel);
            Assert.Equal("application/vnd.customer", link.Type);
        }
        
        [Fact]
        public void BuildsCanonicalLinkWithDefaultUriTemplate()
        {
            var location = new Location {Id = 54};
            var link = LinkHelper.GetCanonicalLinkForModel(location);
            Assert.NotNull(link);
            Assert.Equal("/locations/54", link.Href);
            Assert.Equal("self", link.Rel);
            Assert.Equal("application/vnd.location", link.Type);
        }

        [Fact]
        public void GetsRootLinks()
        {
            var links = LinkHelper.GetRootLinks();
            var link = links.Single(l => l.GetHandlerType() == typeof (LocationsHandler));
            Assert.Equal("/locations", link.Href);
            Assert.Equal("locations", link.Rel);
            Assert.Equal("application/vnd.list.location", link.Type);
        }
    }

    public class Customer
    {
        public int Id { get; set; }
    }

    public class Location
    {
        public int Id { get; set; }
    }

    [LinksFrom(typeof(Customer), "/customers/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")]
    public class CustomerOrdersHandler
    {
        
    }

    [Canonical(typeof(Customer), "/customer/{Id}", Type = "application/vnd.customer")]
    public class CustomerHandler
    {
        
    }
    
    [UriTemplate("/locations/{Id}")]
    [Canonical(typeof(Location), Type = "application/vnd.location")]
    public class LocationHandler
    {
        
    }

    [UriTemplate("/customers/{Id}/location")]
    [LinksFrom(typeof(Customer), Rel = "customer.location", Type = "application/vnd.location")]
    public class CustomerLocationHandler
    {
        
    }

    [UriTemplate("/locations")]
    [Root(Rel = "locations", Type = "application/vnd.list.location")]
    public class LocationsHandler
    {
        
    }
}