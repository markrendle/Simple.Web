using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simple.Web.Behaviors;
using Simple.Web.Routing;
using Xunit;

namespace Simple.Web.Tests.Routing
{
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
            var allTypes = table.GetAllTypes();
            Assert.Contains(typeof(GetThingConstraint<Entity>), allTypes);
            Assert.Contains(typeof(GetThingConstraint<Exorcist>), allTypes);
        }

        [Fact]
        public void FiltersHandlerByContentType()
        {
            var builder = new RoutingTableBuilder(typeof (IGet));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string> variables;
            var actual = table.Get("/spaceship", out variables, "", new[] {"image/png"});
            Assert.Equal(typeof(GetSpaceshipImage), actual);
        }

        [Fact]
        public void FiltersHandlerByWildcardContentType()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string> variables;
            var actual = table.Get("/spaceship", out variables, "", new[] { "*/*" });
            Assert.Equal(typeof(GetSpaceship), actual);
        }

        [Fact]
        public void FiltersHandlerByNoContentType()
        {
            var builder = new RoutingTableBuilder(typeof(IGet));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string> variables;
            var actual = table.Get("/spaceship", out variables);
            Assert.Equal(typeof(GetSpaceship), actual);
        }

        [Fact]
        public void FindsGetWithUltimateBaseClassNoInterface()
        {
            var builder = new RoutingTableBuilder(typeof (IGetAsync));
            var table = builder.BuildRoutingTable();
            IDictionary<string, string> variables;
            var actual = table.Get("/top/bottom", out variables, "", new string[0]);
            Assert.Equal(typeof(Bottom), actual);
        }

        [Fact]
        public void FindsTypeWithTwoHttpInterfaces()
        {
            var putTable = Application.BuildRoutingTable("PUT");
            IDictionary<string, string> variables;
            var actualPut = putTable.Get("/dualmethod", out variables);
            Assert.Equal(typeof(DualMethod), actualPut);

            var patchTable = Application.BuildRoutingTable("PATCH");
            var actualPatch = patchTable.Get("/dualmethod", out variables);
            Assert.Equal(typeof(DualMethod), actualPatch);
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
    [RegexGenericResolver("T", @"^Simple\.Web\.Tests\.Routing\.")]
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

    [UriTemplate("/top")]
    public abstract class Top
    {
    }

    public abstract class Middle : Top, IGetAsync
    {
        public abstract Task<Status> Get();
    }

    [UriTemplate("/bottom")]
    public class Bottom : Middle
    {
        public override Task<Status> Get()
        {
            throw new NotImplementedException();
        }
    }

    [UriTemplate("/dualmethod")]
    public class DualMethod : IPutAsync<Entity>, IPatchAsync<Entity>
    {
        public Task<Status> Put(Entity input)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Patch(Entity input)
        {
            throw new NotImplementedException();
        }
    }
}
