namespace Simple.Web.JsonNet.Tests
{
    using System;
    using System.Collections.Generic;
    using MediaTypeHandling;
    using Newtonsoft.Json.Linq;
    using TestHelpers;
    using TestHelpers.Sample;
    using Xunit;

    public class SimpleLinkFormatterTests
    {
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
            var target = new JsonMediaTypeHandler
            {
                JsonLinksFormatter = new SimpleJsonLinksFormatter()
            };
            string json;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Customer>(content, stream).Wait();
                json = stream.StringValue;
            }
            var jobj = JObject.Parse(json);
            var links = jobj["_links"] as JObject;
            Assert.NotNull(links);
            Assert.Equal(links["customer.orders"].ToString(), "/customer/42/orders");
            Assert.Equal(links["self"].ToString(), "/customer/42");
        }

        [Fact]
        public void PicksUpPathFromThing()
        {
            const string thingLink =
                @"{""title"":null,""href"":""/things?path=%2Ffoo%2Fbar"",""rel"":""self"",""type"":""application/json""}";

            var content = new Content(new Uri("http://test.com/foo/bar"), new ThingHandler(),
                new Thing {Path = "/foo/bar"});
            var target = new JsonMediaTypeHandler
            {
                JsonLinksFormatter = new SimpleJsonLinksFormatter()
            };
            string json;
            using (var stream = new StringBuilderStream())
            {
                target.Write<Thing>(content, stream).Wait();
                json = stream.StringValue;
            }
            var jobj = JObject.Parse(json);
            var links = jobj["_links"] as JObject;
            Assert.NotNull(links);
            Assert.Equal(links["self"].ToString(), "/things?path=%2Ffoo%2Fbar");
        }
    }
}