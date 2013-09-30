namespace Simple.Web.JsonNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using Simple.Web.MediaTypeHandling;
    using Simple.Web.TestHelpers;
    using Simple.Web.TestHelpers.Sample;

    using Xunit;

    public class JsonMediaTypeHandlerWithDeepLinksTests
    {
        [Fact]
        public void AddsSelfLinkToChildCollectionItems()
        {
            var customer = new Customer { Id = 42, Orders = new List<Order> { new Order { CustomerId = 42, Id = 54 } } };
            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), customer);
            var target = new JsonMediaTypeHandlerWithDeepLinks();

            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);
            var jobj = JObject.Parse(actual);
            var orders = jobj["orders"] as JArray;
            Assert.NotNull(orders);
            var order = orders[0] as JObject;
            Assert.NotNull(order);
            var links = order["links"] as JArray;
            Assert.NotNull(links);
            var self = links.FirstOrDefault(jt => jt["rel"].Value<string>() == "self");
            Assert.NotNull(self);
            Assert.Equal("/order/54", self["href"].Value<string>());
            Assert.Equal("application/vnd.order+json", self["type"].Value<string>());
        }

        [Fact]
        public void PicksUpContactsLinkFromCustomer()
        {
            const string contactsLink =
                @"{""href"":""/customer/42/contacts"",""rel"":""customer.contacts"",""title"":null,""type"":""application/vnd.contact+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new Customer { Id = 42 });
            var target = new JsonMediaTypeHandlerWithDeepLinks();
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
        public void PicksUpOrdersLinkFromCustomer()
        {
            const string idProperty = @"""id"":42";
            const string ordersLink =
                @"{""href"":""/customer/42/orders"",""rel"":""customer.orders"",""title"":null,""type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""href"":""/customer/42"",""rel"":""self"",""title"":null,""type"":""application/vnd.customer+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new Customer { Id = 42 });
            var target = new JsonMediaTypeHandlerWithDeepLinks();
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
        public void PicksUpOrdersLinkFromCustomers()
        {
            const string idProperty = @"""id"":42";
            const string ordersLink =
                @"{""href"":""/customer/42/orders"",""rel"":""customer.orders"",""title"":null,""type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""href"":""/customer/42"",""rel"":""self"",""title"":null,""type"":""application/vnd.customer+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new[] { new Customer { Id = 42 } });
            var target = new JsonMediaTypeHandlerWithDeepLinks();
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
                @"{""href"":""/things?path=%2Ffoo%2Fbar"",""rel"":""self"",""title"":null,""type"":""application/json""}";

            var content = new Content(new Uri("http://test.com/foo/bar"), new ThingHandler(), new Thing { Path = "/foo/bar" });
            var target = new JsonMediaTypeHandlerWithDeepLinks();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Thing>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);
            Assert.Contains(thingLink, actual);
        }

        [Fact]
        public void SerializesCyrillicText()
        {
            const string russian = "Мыа алиё лаборамюз ед, ведят промпта элыктрам квюо ты.";
            var content = new Content(new Uri("http://test.com/customer/42"), new ThingHandler(), new Thing { Path = russian });
            var target = new JsonMediaTypeHandlerWithDeepLinks();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Thing>(content, stream).Wait();
                actual = stream.StringValue;
            }

            Assert.Contains(russian, actual);
        }
    }
}