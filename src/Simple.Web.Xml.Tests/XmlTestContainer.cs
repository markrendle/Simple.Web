using System.Xml.Linq;
using Simple.Web.DependencyInjection;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml.Tests
{
    public class XmlTestContainer : ISimpleContainer
    {
        private readonly Scope _scope = new Scope();

        public ISimpleContainerScope BeginScope()
        {
            return _scope;
        }

        private class Scope : ISimpleContainerScope
        {
            public void Dispose()
            {
            }

            public T Get<T>()
            {
                var requestedType = typeof (T);
                if (requestedType == typeof (IMediaConverter<Order, XElement>))
                {
                    return (T) (object) new OrderConverter();
                }
                if (requestedType == typeof (IMediaConverter<Customer, XElement>))
                {
                    return (T) (object) new CustomerConverter();
                }
                throw new System.NotImplementedException();
            }
        }
    }
}