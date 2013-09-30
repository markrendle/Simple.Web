namespace Simple.Web.Xml.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using Simple.Web.MediaTypeHandling;
    using Simple.Web.TestHelpers;
    using Simple.Web.TestHelpers.Sample;
    using Simple.Web.TestHelpers.Xml;

    using Xunit;

    public class ExplicitXmlContentTypeHandlerTests
    {
        public ExplicitXmlContentTypeHandlerTests()
        {
            SimpleWeb.Configuration.Container = new XmlTestContainer();
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new Customer { Id = 42 });
            var target = new ExplicitXmlMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);

            const string expected =
                "<Customer Id='42'>" + "  <link href='/customer/42/contacts' rel='customer.contacts' type='application/vnd.contact+xml' />" +
                "  <link href='/customer/42/orders' rel='customer.orders' type='application/vnd.list.order+xml' />" +
                "  <link href='/customer/42' rel='self' type='application/vnd.customer+xml' />" + "</Customer>";

            XElement.Parse(actual).ShouldEqual(expected);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomers()
        {
            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new[] { new Customer { Id = 42 } });
            var target = new ExplicitXmlMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<IEnumerable<Customer>>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);

            const string expected =
                "<Customers>" + "  <Customer Id='42'>" +
                "    <link href='/customer/42/contacts' rel='customer.contacts' type='application/vnd.contact+xml' />" +
                "    <link href='/customer/42/orders' rel='customer.orders' type='application/vnd.list.order+xml' xmlns='' />" +
                "    <link href='/customer/42' rel='self' type='application/vnd.customer+xml' xmlns='' />" + "  </Customer>" +
                "</Customers>";

            XElement.Parse(actual).ShouldEqual(expected);
        }

        [Fact]
        public void SerializesOrder()
        {
            var content = new Content(new Uri("http://test.com/order/42"), new OrderHandler(), new Order { Id = 54, CustomerId = 42 });
            var target = new ExplicitXmlMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Order>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);

            const string expected =
                "<Order Id='54' CustomerId='42'>" + "  <link href='/order/54' rel='self' type='application/vnd.order+xml' />" + "</Order>";

            XElement.Parse(actual).ShouldEqual(expected);
        }
    }
}