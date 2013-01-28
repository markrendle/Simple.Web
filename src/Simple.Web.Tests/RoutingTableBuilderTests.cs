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
        public void FindsGenericHandlerUsingRegexResolver()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetThingRegex<Entity>), table.GetAllTypes());
            Assert.Contains(typeof(GetThingRegex<Exorcist>), table.GetAllTypes());
        }
        
        [Fact]
        public void FindsGenericHandlerUsingExplicitResolver()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetThingExplicit<Entity>), table.GetAllTypes());
            Assert.Contains(typeof(GetThingExplicit<Exorcist>), table.GetAllTypes());
        }
        
        [Fact]
        public void FindsGenericHandlerUsingConstraints()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetThingConstraint<Entity>), table.GetAllTypes());
            Assert.Contains(typeof(GetThingConstraint<Exorcist>), table.GetAllTypes());
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

    [UriTemplate("/regex/{T}/{Id}")]
    [RegexGenericResolver("T", "^Simple\\.Web\\.Tests\\.")]
    public class GetThingRegex<T> : IGet, IOutput<T>
    {
        public Status Get()
        {
            return Status.OK;
        }

        public T Output { get; private set; }
    }
    
    [UriTemplate("/explicit/{T}/{Id}")]
    [ExplicitGenericResolver("T", typeof(Entity), typeof(Exorcist))]
    public class GetThingExplicit<T> : IGet, IOutput<T>
    {
        public Status Get()
        {
            return Status.OK;
        }

        public T Output { get; private set; }
    }

    [UriTemplate("/constraint/{T}/{Id}")]
    public class GetThingConstraint<T> : IGet, IOutput<T> where T : IHorror
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

    public class Entity : IHorror
    {
        
    }
    
    public class Exorcist : IHorror
    {
        
    }

    public interface IHorror
    {
        
    }
}
