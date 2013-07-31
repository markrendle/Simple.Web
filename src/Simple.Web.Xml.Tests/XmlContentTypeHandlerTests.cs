using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Xml.Tests
{
    using System.IO;
    using System.Xml.Linq;
    using Links;
    using MediaTypeHandling;
    using TestHelpers;
    using Xunit;

    public class XmlContentTypeHandlerTests
    {
        [Fact]
        public void SerializesOrder()
        {
            var content = new Content(new Uri("http://test.com/order/42"), new OrderHandler(),
                                      new Order {Id = 54, CustomerId = 42});
            var target = new XmlMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write<Order>(content, stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
            }
            Assert.NotNull(actual);

            const string expected = "<Order xmlns='http://schemas.datacontract.org/2004/07/Simple.Web.Xml.Tests'" +
                                    "       xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>" +
                                    "  <CustomerId>42</CustomerId>" +
                                    "  <Id>54</Id>" +
                                    "</Order>";

            XElement.Parse(actual).ShouldEqual(expected);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(),
                                      new Customer {Id = 42});
            var target = new XmlMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write<Customer>(content, stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
            }
            Assert.NotNull(actual);

            const string expected = "<?xml version='1.0' encoding='utf-8'?>" +
                                    "<Customer xmlns='http://schemas.datacontract.org/2004/07/Simple.Web.Xml.Tests'" +
                                    "          xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>" +
                                    "  <Id>42</Id>" +
                                    "  <link href='/customer/42/orders' rel='customer.orders' type='application/vnd.list.order+xml' xmlns='' />" +
                                    "  <link href='/customer/42' rel='self' type='application/vnd.customer+xml' xmlns='' />" +
                                    "</Customer>";

            XElement.Parse(actual).ShouldEqual(expected);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomers()
        {
            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(),
                                      new[] {new Customer {Id = 42}});
            var target = new XmlMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write<IEnumerable<Customer>>(content, stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
            }
            Assert.NotNull(actual);

            const string expected = "<?xml version='1.0' encoding='utf-8'?>" +
                                    "<Customers>" +
                                    "  <Customer xmlns='http://schemas.datacontract.org/2004/07/Simple.Web.Xml.Tests'" +
                                    "            xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>" +
                                    "    <Id>42</Id>" +
                                    "    <link href='/customer/42/orders' rel='customer.orders' type='application/vnd.list.order+xml' xmlns='' />" +
                                    "    <link href='/customer/42' rel='self' type='application/vnd.customer+xml' xmlns='' />" +
                                    "  </Customer>" +
                                    "</Customers>";

            XElement.Parse(actual).ShouldEqual(expected);
        }
    }

    [LinksFrom(typeof (Customer), "/customer/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")
    ]
    public class CustomerOrders
    {
    }

    [LinksFrom(typeof (Customer), "/customer/{Id}", Rel = "self", Type = "application/vnd.customer")]
    public class CustomerHandler
    {
    }

    [LinksFrom(typeof (IEnumerable<Customer>), "/customers", Rel = "self", Type = "application/vnd.list.customer")]
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