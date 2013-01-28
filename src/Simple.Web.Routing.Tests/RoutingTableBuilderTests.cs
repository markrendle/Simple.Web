namespace Simple.Web.Routing.Tests
{
    using System;
    using System.Collections.Generic;
    using Behaviors;
    using Routing;
    using Xunit;
    using System.Linq;

    public class RoutingTableBuilderTests
    {
        [Fact]
        public void FindsIGetType()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetFoo), table.GetAllTypes());
        }

        [Fact]
        public void FindsIGetTypeWhereIGetIsOnBaseClass()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(Bar), table.GetAllTypes());
        }

        [Fact]
        public void FindsGenericHandlerUsingRegexResolver()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            var actualTypes = table.GetAllTypes().ToArray();
            Assert.Contains(typeof(GetThingRegex<Entity>), actualTypes);
            Assert.Contains(typeof(GetThingRegex<Exorcist>), actualTypes);
        }

        [Fact]
        public void FindsGenericHandlerUsingExplicitResolver()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetThingExplicit<Entity>), table.GetAllTypes());
            Assert.Contains(typeof(GetThingExplicit<Exorcist>), table.GetAllTypes());
        }

        [Fact]
        public void FindsGenericHandlerUsingConstraints()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            Assert.Contains(typeof(GetThingConstraint<Entity>), table.GetAllTypes());
            Assert.Contains(typeof(GetThingConstraint<Exorcist>), table.GetAllTypes());
        }

        [Fact]
        public void FiltersHandlerByContentType()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string[]> variables;
            var actual = table.Get("/spaceship", "", new[] {"image/png"}, out variables);
            Assert.Equal(typeof(GetSpaceshipImage), actual);
        }

        [Fact]
        public void FiltersHandlerByWildcardContentType()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string[]> variables;
            var actual = table.Get("/spaceship", "", new[] { "*/*" }, out variables);
            Assert.Equal(typeof(GetSpaceship), actual);
        }

        [Fact]
        public void FiltersHandlerByNoContentType()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string[]> variables;
            var actual = table.Get("/spaceship", "", new string[0], out variables);
            Assert.Equal(typeof(GetSpaceship), actual);
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
    [RegexGenericResolver("T", @"^Simple\.Web\.Routing\.Tests\.")]
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

    [UriTemplate("/spaceship")]
    public class GetSpaceship : IGet
    {
        public Status Get()
        {
            throw new NotImplementedException();
        }
    }
    
    [UriTemplate("/spaceship")]
    [RespondsWith("image/png")]
    public class GetSpaceshipImage : IGet
    {
        public Status Get()
        {
            throw new NotImplementedException();
        }
    }
}
