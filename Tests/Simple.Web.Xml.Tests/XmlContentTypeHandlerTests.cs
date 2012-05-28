using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Xml.Tests
{
    using System.IO;
    using System.Xml.Linq;
    using ContentTypeHandling;
    using Links;
    using TestHelpers;
    using Xunit;

    public class XmlContentTypeHandlerTests
    {
        [Fact]
        public void SerializesOrder()
        {
            var content = new Content(new OrderHandler(), new Order {Id = 54, CustomerId = 42});
            var target = new XmlContentTypeHandler();
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

            var root = XElement.Parse(actual);
            Assert.Equal("Order", root.Name.LocalName);
            XElement id = root.Element(root.GetDefaultNamespace() + "Id");
            Assert.NotNull(id);
            Assert.Equal("54", id.Value);
            
            XElement customerId = root.Element(root.GetDefaultNamespace() + "CustomerId");
            Assert.NotNull(customerId);
            Assert.Equal("42", customerId.Value);
        }
        
        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            var content = new Content(new CustomerHandler(), new Customer { Id = 42 });
            var target = new XmlContentTypeHandler();
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

            var root = XElement.Parse(actual);
            Assert.Equal("Customer", root.Name.LocalName);
            XElement id = root.Element(root.GetDefaultNamespace() + "Id");
            Assert.NotNull(id);
            Assert.Equal("42", id.Value);
            var links = root.Elements("link").ToList();
            Assert.Equal(2, links.Count);
        }
        
        [Fact]
        public void PicksUpOrdersLinkFromCustomers()
        {
            var content = new Content(new CustomerHandler(), new[] {new Customer {Id = 42}});
            var target = new XmlContentTypeHandler();
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

            var root = XElement.Parse(actual);
            Assert.Equal("Customers", root.Name.LocalName);
            var customer = root.Elements().FirstOrDefault(x => x.Name.LocalName == "Customer");
            XElement id = customer.Element(customer.GetDefaultNamespace() + "Id");
            Assert.NotNull(id);
            Assert.Equal("42", id.Value);
            var links = customer.Elements("link").ToList();
            Assert.Equal(2, links.Count);
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

    public class OrderHandler
    {
        
    }

    public class Customer
    {
        public int Id { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
    }
}
