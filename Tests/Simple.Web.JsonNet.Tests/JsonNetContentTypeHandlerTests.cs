using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.JsonNet.Tests
{
    using System.IO;
    using Links;
    using MediaTypeHandling;
    using TestHelpers;
    using Xunit;

    public class JsonNetContentTypeHandlerTests
    {
        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            const string idProperty = @"""id"":42";
            const string ordersLink =
                @"{""title"":null,""href"":""/customer/42/orders"",""rel"":""customer.orders"",""type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""title"":null,""href"":""/customer/42"",""rel"":""self"",""type"":""application/vnd.customer+json""}";

            var content = new Content(new CustomerHandler(), new Customer { Id = 42 });
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write(content, stream).Wait();
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
        public void PicksUpContactsLinkFromCustomer()
        {
            const string contactsLink =
                @"{""title"":null,""href"":""/customer/42/contacts"",""rel"":""customer.contacts"",""type"":""application/json""}";

            var content = new Content(new CustomerHandler(), new Customer { Id = 42 });
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write(content, stream).Wait();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
            }
            Assert.NotNull(actual);
            Assert.Contains(contactsLink, actual);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomers()
        {
            const string idProperty = @"""id"":42";
            const string ordersLink =
                @"{""title"":null,""href"":""/customer/42/orders"",""rel"":""customer.orders"",""type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""title"":null,""href"":""/customer/42"",""rel"":""self"",""type"":""application/vnd.customer+json""}";

            var content = new Content(new CustomerHandler(), new[] { new Customer { Id = 42 } });
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write(content, stream).Wait();
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

    [UriTemplate("/customer")]
    public abstract class CustomerBase
    {
        
    }

    [UriTemplate("/{Id}/contacts")]
    [LinksFrom(typeof(Customer), Rel = "customer.contacts")]
    public class CustomerContacts: CustomerBase
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
