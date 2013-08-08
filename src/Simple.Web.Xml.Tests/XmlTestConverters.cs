using System.Xml.Linq;

namespace Simple.Web.Xml.Tests
{
    internal class OrderConverter : IConvertXmlFor<Order>
    {
        public Order FromXml(XElement wireFormat)
        {
            throw new System.NotImplementedException();
        }

        public XElement ToXml(Order value)
        {
            return new XElement("Order",
                                new XElement("CustomerId", value.CustomerId),
                                new XElement("Id", value.Id));
        }
    }

    internal class CustomerConverter : IConvertXmlFor<Customer>
    {
        public Customer FromXml(XElement wireFormat)
        {
            throw new System.NotImplementedException();
        }

        public XElement ToXml(Customer value)
        {
            return new XElement("Customer",
                                new XElement("Id", value.Id));
        }
    }
}