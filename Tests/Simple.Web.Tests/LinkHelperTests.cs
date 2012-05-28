namespace Simple.Web.Tests
{
    using System.Linq;
    using Helpers;
    using Links;
    using Xunit;

    public class LinkHelperTests
    {
        [Fact]
        public void BuildsLink()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetLinksForModel(customer).SingleOrDefault(l => l.GetHandlerType() == typeof(CustomerOrdersHandler));
            Assert.NotNull(link);
            Assert.Equal("/customers/42/orders", link.Href);
            Assert.Equal("customer.orders", link.Rel);
            Assert.Equal("application/vnd.list.order", link.Type);
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
    }

    public class Customer
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
}