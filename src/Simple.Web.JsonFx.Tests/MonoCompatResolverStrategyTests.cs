namespace Simple.Web.JsonFx.Tests
{
    using System;
    using System.Collections.Generic;
    using Links;
    using MediaTypeHandling;
    using TestHelpers;
    using TestHelpers.Sample;
    using Xunit;

    public class MonoCompatResolverStrategyTests
    {
        [Fact]
        public void SerializingListToJsonHasExpectedData()
        {
            const string expectedString = "{\"Customers\":[{\"Id\":42,\"Orders\":null}]}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomersListHandler(), new CustomerList() { Customers = new List<Customer>() { new Customer { Id = 42 } } });
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new StringBuilderStream())
            {
                target.Write<CustomerList>(content, stream).Wait();
                actual = stream.StringValue;
            }
            Assert.NotNull(actual);      
            Assert.Equal(expectedString, actual);
        }

        [LinksFrom(typeof(IEnumerable<CustomerList>), "/allcustomers", Rel = "self", Type = "application/vnd.list.customer")]
        public class CustomersListHandler
        {
        }

        public class CustomerList
        {
            public IList<Customer> Customers { get; set; }
        }
    }
}
