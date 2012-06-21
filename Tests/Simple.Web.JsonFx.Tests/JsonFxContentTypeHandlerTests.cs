using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.JsonFx.Tests
{
    using System.IO;
    using Links;
    using MediaTypeHandling;
    using TestHelpers;
    using Xunit;

    public class JsonFxContentTypeHandlerTests
    {
        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            const string idProperty = @"""Id"":42";
            const string ordersLink =
                @"{""Title"":null,""Href"":""/customer/42/orders"",""Rel"":""customer.orders"",""Type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""Title"":null,""Href"":""/customer/42"",""Rel"":""self"",""Type"":""application/vnd.customer+json""}]}";

            var content = new Content(new CustomerHandler(), new Customer {Id = 42});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write(content, stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
            }
            Assert.NotNull(actual);
            Assert.Contains(idProperty, actual);
            Assert.Contains(ordersLink, actual);
            Assert.Contains(selfLink, actual);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomers()
        {
            const string idProperty = @"""Id"":42";
            const string ordersLink =
                @"{""Title"":null,""Href"":""/customer/42/orders"",""Rel"":""customer.orders"",""Type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""Title"":null,""Href"":""/customer/42"",""Rel"":""self"",""Type"":""application/vnd.customer+json""}]}";

            var content = new Content(new CustomerHandler(), new[] {new Customer { Id = 42 }});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write(content, stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
            }
            Assert.NotNull(actual);
            Assert.Contains(idProperty, actual);
            Assert.Contains(ordersLink, actual);
            Assert.Contains(selfLink, actual);
        }
    }

    [LinksFrom(typeof(Customer), "/customer/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")]
    public class CustomerOrders
    {
        
    }

    [LinksFrom(typeof(Customer), "/customer/{Id}", Rel = "self", Type = "application/vnd.customer")]
    public class CustomerHandler
    {
        
    }

    [LinksFrom(typeof(IEnumerable<Customer>), "/customers", Rel = "self", Type = "application/vnd.list.customer")]
    public class CustomersHandler
    {
    }

    public class Customer
    {
        public int Id { get; set; }
    }
}
