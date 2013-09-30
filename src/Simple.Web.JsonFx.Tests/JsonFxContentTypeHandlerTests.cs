namespace Simple.Web.JsonFx.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Simple.Web.MediaTypeHandling;
    using Simple.Web.TestHelpers;
    using Simple.Web.TestHelpers.Sample;

    using Xunit;

    public class JsonFxContentTypeHandlerTests
    {
        [Fact]
        public void ParsesJasnWithEnum()
        {
            var content = "{ \"AnEnum\": \"Ian\" }";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            var handler = new JsonMediaTypeHandler();

            var obj = handler.Read<EnumCustomer>(stream).Result;

            stream.Close();

            Assert.IsType<EnumCustomer>(obj);
            Assert.NotNull(obj);
            Assert.Equal(MyEnum.Ian, obj.AnEnum);
        }

        [Fact]
        public void PicksUpOrdersLinkFromCustomer()
        {
            const string idProperty = @"""Id"":42";
            const string ordersLink =
                @"{""Title"":null,""Href"":""/customer/42/orders"",""Rel"":""customer.orders"",""Type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""Title"":null,""Href"":""/customer/42"",""Rel"":""self"",""Type"":""application/vnd.customer+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new Customer { Id = 42 });
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
        public void PicksUpOrdersLinkFromCustomers()
        {
            const string idProperty = @"""Id"":42";
            const string ordersLink =
                @"{""Title"":null,""Href"":""/customer/42/orders"",""Rel"":""customer.orders"",""Type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""Title"":null,""Href"":""/customer/42"",""Rel"":""self"",""Type"":""application/vnd.customer+json""}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new[] { new Customer { Id = 42 } });
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
        public void WritesJsonWithEnum()
        {
            var content = new Content(new Uri("http://test.com/EnumCustomer/42"),
                                      new EnumCustomerHandler(),
                                      new[] { new EnumCustomer { AnEnum = MyEnum.Ian } });
            var target = new JsonMediaTypeHandler();

            string actual;

            using (var stream = new StringBuilderStream())
            {
                target.Write<EnumCustomer>(content, stream).Wait();
                actual = stream.StringValue;
            }

            Assert.NotNull(actual);
        }
    }
}