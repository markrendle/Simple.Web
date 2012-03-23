using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Tests
{
    using Xunit;

    public class RoutingTableBuilderTests
    {
        [Fact]
        public void FindsIGetType()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetFoo), table.GetAllTypes());
        }
    }

    [UriTemplate("/foo")]
    public class GetFoo : IGet
    {
        public Status Get()
        {
            throw new NotImplementedException();
        }

        public object Output
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class GetGenericFoo : IGet, IOutput<object>
    {
        public Status Get()
        {
            throw new NotImplementedException();
        }

        public object Output
        {
            get { throw new NotImplementedException(); }
        }
    }
}
