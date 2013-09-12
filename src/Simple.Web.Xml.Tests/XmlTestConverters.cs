using System;
using System.Xml.Linq;
using Simple.Web.TestHelpers.Sample;

namespace Simple.Web.Xml.Tests
{
    internal class OrderConverter : XmlConverter<Order>
    {
        public override Order FromXml(XElement wireFormat)
        {
            throw new NotImplementedException();
        }

        public override XElement ToXml(Order value)
        {
            return new XElement("Order",
                                new XElement("CustomerId", value.CustomerId),
                                new XElement("Id", value.Id));
        }
    }

    internal class CustomerConverter : XmlConverter<Customer>
    {
        public override Customer FromXml(XElement wireFormat)
        {
            throw new NotImplementedException();
        }

        public override XElement ToXml(Customer value)
        {
            return new XElement("Customer",
                                new XElement("Id", value.Id));
        }
    }
}