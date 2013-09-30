namespace Simple.Web.Xml.Tests
{
    using System;

    using Simple.Web.DependencyInjection;
    using Simple.Web.TestHelpers.Sample;

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
                Type requestedType = typeof(T);
                return (T)Get(requestedType);
            }

            public object Get(Type objectType)
            {
                if (objectType == typeof(XmlConverter<Order>))
                {
                    return new OrderConverter();
                }
                if (objectType == typeof(XmlConverter<Customer>))
                {
                    return new CustomerConverter();
                }
                throw new NotImplementedException();
            }
        }
    }
}