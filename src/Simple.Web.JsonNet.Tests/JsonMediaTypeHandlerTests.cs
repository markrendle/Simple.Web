namespace Simple.Web.JsonNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MediaTypeHandling;
    using Newtonsoft.Json.Linq;
    using TestHelpers;
    using TestHelpers.Sample;
    using Xunit;

    public class JsonMediaTypeHandlerTests
    {
        [Fact]
        public void SerializesCyrillicText()
        {
            const string russian = "Мыа алиё лаборамюз ед, ведят промпта элыктрам квюо ты.";
            var content = new Content(new Uri("http://test.com/customer/42"), new ThingHandler(),
                                      new Thing {Path = russian});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Thing>(content, stream).Wait();
                actual = stream.StringValue;
            }

            Assert.Contains(russian, actual);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            const string idProperty = @"""id"":42";
            const string ordersLink =
                @"{""title"":null,""href"":""/customer/42/orders"",""rel"":""customer.orders"",""type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""title"":null,""href"":""/customer/42"",""rel"":""self"",""type"":""application/vnd.customer+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(),
                                      new Customer {Id = 42});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
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
                @"{""title"":null,""href"":""/customer/42/contacts"",""rel"":""customer.contacts"",""type"":""application/vnd.contact+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(),
                                      new Customer {Id = 42});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
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

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(),
                                      new[] {new Customer {Id = 42}});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<IEnumerable<Customer>>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);
            Assert.Contains(idProperty, actual);
            Assert.Contains(ordersLink, actual);
            Assert.Contains(selfLink, actual);
        }

        [Fact]
        public void PicksUpPathFromThing()
        {
            const string thingLink =
                @"{""title"":null,""href"":""/things?path=%2Ffoo%2Fbar"",""rel"":""self"",""type"":""application/json""}";

            var content = new Content(new Uri("http://test.com/foo/bar"), new ThingHandler(),
                                      new Thing {Path = "/foo/bar"});
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Thing>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);
            Assert.Contains(thingLink, actual);
        }
    }
}