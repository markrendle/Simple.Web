namespace Simple.Web.JsonFx.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Links;
    using MediaTypeHandling;
    using TestHelpers;
    using Xunit;

    public class MonoCompatResolverStrategyTests
    {
        [Fact]
        public void SerializingListToJsonHasExpectedData()
        {
            const string expectedString = "{\"Customers\":[{\"Id\":42}]}";

            var content = new Content(new Uri("http://test.com/customer/42"), new CustomersListHandler(), new CustomerList() { Customers = new List<Customer>() { new Customer { Id = 42 } } });
            var target = new JsonMediaTypeHandler();
            string actual;
            using (var stream = new NonClosingMemoryStream(new MemoryStream()))
            {
                target.Write<CustomerList>(content, stream).Wait();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    actual = reader.ReadToEnd();
                }
                stream.ForceDispose();
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
