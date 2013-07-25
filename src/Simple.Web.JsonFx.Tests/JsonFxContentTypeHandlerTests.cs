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

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new Customer { Id = 42 });
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
        public void PicksUpOrdersLinkFromCustomers()
        {
            const string idProperty = @"""Id"":42";
            const string ordersLink =
                @"{""Title"":null,""Href"":""/customer/42/orders"",""Rel"":""customer.orders"",""Type"":""application/vnd.list.order+json""}";
            const string selfLink =
                @"{""Title"":null,""Href"":""/customer/42"",""Rel"":""self"",""Type"":""application/vnd.customer+json""}]}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomerHandler(), new[] { new Customer { Id = 42 } });
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
        public void WritesJsonWithEnum()
        {
            var content = new Content(new Uri("http://test.com/EnumCustomer/42"), new EnumCustomerHandler(), new[] { new EnumCustomer() { AnEnum = MyEnum.Ian } });
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
        }

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

            var obj = handler.Read(stream, typeof(EnumCustomer));

            stream.Close();

            Assert.IsType<EnumCustomer>(obj);
            Assert.NotNull(obj);
            Assert.Equal(MyEnum.Ian, (obj as EnumCustomer).AnEnum);
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

    [LinksFrom(typeof(EnumCustomer), "/enumcustomer/{Id}", Rel = "self", Type = "application/vnd.enum.customer")]
    public class EnumCustomerHandler
    {

    }

    public class Customer
    {
        public int Id { get; set; }
    }

    public enum MyEnum
    {
        Ian,
        Franc
    }

    public class EnumCustomer
    {
        public MyEnum AnEnum { get; set; }
    }
}
