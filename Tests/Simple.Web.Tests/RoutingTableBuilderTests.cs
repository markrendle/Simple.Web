using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Tests
{
    using Behaviors;
    using Routing;
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
 
        [Fact]
        public void FindsIGetTypeWhereIGetIsOnBaseClass()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(Bar), table.GetAllTypes());
        }

        [Fact]
        public void FindsGenericHandler()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetThing<Entity>), table.GetAllTypes());
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

    [UriTemplate("/{T}/{Id}")]
    [RegexGenericResolver("T", "^Simple\\.Web\\.Tests\\.")]
    public class GetThing<T> : IGet, IOutput<T>
    {
        public Status Get()
        {
            return Status.OK;
        }

        public T Output { get; private set; }
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

    public abstract class BaseBar : IGet
    {
        public Status Get()
        {
            throw new NotImplementedException();
        }
    }

    [UriTemplate(("/bar"))]
    public class Bar : BaseBar
    {
        
    }

    public class Entity
    {
        
    }
}
