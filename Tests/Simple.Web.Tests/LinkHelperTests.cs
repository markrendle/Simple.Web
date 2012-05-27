namespace Simple.Web.Tests
{
    using System.Linq;
    using Helpers;
    using Xunit;

    public class LinkHelperTests
    {
        [Fact]
        public void BuildsLink()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetLinksForModel(customer).SingleOrDefault();
            Assert.NotNull(link);
            Assert.Equal("/customers/42/orders", link.Href);
            Assert.Equal("customer.orders", link.Rel);
            Assert.Equal("application/vnd.list.order", link.Type);
        }
    }

    public class Customer
    {
        public int Id { get; set; }
    }

    [LinksFrom(typeof(Customer), "/customers/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")]
    public class CustomerOrders
    {
        
    }
}