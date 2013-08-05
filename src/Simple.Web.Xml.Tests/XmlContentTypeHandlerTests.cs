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
            using (var stream = new StringBuilderStream())
            {
                target.Write<Order>(content, stream).Wait();
                actual = stream.StringValue;
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
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
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
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
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
}