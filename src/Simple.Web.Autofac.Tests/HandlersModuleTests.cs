using System.Reflection;
using Autofac;
using Xunit;

namespace Simple.Web.Autofac.Tests
{
    public class HandlersModuleTests
    {
        private readonly IContainer _container;

        public HandlersModuleTests()
        {
            var builder = new ContainerBuilder();

            builder.Register(c => Status.Created).AsSelf();
            builder.RegisterHandlersInAssembly(Assembly.GetExecutingAssembly());

            _container = builder.Build();
        }

        [Fact]
        public void module_should_register_generic_handlers()
        {
            Assert.True(_container.IsRegistered<GenericCustomerHandler>());
        }

        [Fact]
        public void module_should_register_nongeneric_handlers()
        {
            Assert.True(_container.IsRegistered<CustomerHandler>());
        }

        public class Customer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class GenericCustomerHandler : IPost<Customer>, IPut<Customer>
        {
            private readonly Status _status;

            public GenericCustomerHandler(Status status)
            {
                _status = status;
            }

            public Status Post(Customer input)
            {
                return _status;
            }

            public Status Put(Customer input)
            {
                return _status;
            }
        }

        public class CustomerHandler : IPost, IPut
        {
            private readonly Status _status;

            public CustomerHandler(Status status)
            {
                _status = status;
            }

            public Status Post()
            {
                return _status;
            }

            public Status Put()
            {
                return _status;
            }
        }
    }
}
