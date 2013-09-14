using System;
using System.Xml.Linq;
using Simple.Web.TestHelpers.Sample;

namespace Simple.Web.Xml.Tests
{
    internal class OrderConverter : XmlConverter<Order>
    {
        internal static readonly OrderConverter Instance = new OrderConverter();

        public override Order FromXml(XElement wireFormat)
        {
            throw new NotImplementedException();
        }

        public override XElement ToXml(Order value)
        {
            return new XElement("Order",
                                new XAttribute("Id", value.Id),
                                new XAttribute("CustomerId", value.CustomerId));
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
            var xml = new XElement("Customer",
                                   new XAttribute("Id", value.Id));
            if (value.Orders != null && value.Orders.Count > 0)
            {
                foreach (Order order in value.Orders)
                {
                    xml.Add(OrderConverter.Instance.ToXml(order));
                }
            }
            return xml;
        }
    }
}