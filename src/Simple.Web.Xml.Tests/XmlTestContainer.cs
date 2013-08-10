namespace Simple.Web.Xml.Tests
{
    using DependencyInjection;
    using TestHelpers.Sample;

    public class XmlTestContainer : ISimpleContainer
    {
        public ISimpleContainerScope BeginScope()
        {
            return new Scope();
        }

        private class Scope : ISimpleContainerScope
        {
            public void Dispose()
            {
            }

            public T Get<T>()
            {
                var requestedType = typeof (T);
                if (requestedType == typeof (IConvertXmlFor<Order>))
                {
                    return (T) (object) new OrderConverter();
                }
                if (requestedType == typeof (IConvertXmlFor<Customer>))
                {
                    return (T) (object) new CustomerConverter();
                }
                throw new System.NotImplementedException();
            }
        }
    }
}