using System.Xml.Linq;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml.Tests
{
    internal class OrderConverter : IMediaConverter<Order, XElement>
    {
        public Order FromWireFormat(XElement wireFormat)
        {
            throw new System.NotImplementedException();
        }

        public Order FromWireFormat(XElement wireFormat, Order loadThis)
        {
            throw new System.NotImplementedException();
        }

        public XElement ToWireFormat(Order value)
        {
            return new XElement("Order",
                                new XElement("CustomerId", value.CustomerId),
                                new XElement("Id", value.Id));
        }
    }

    internal class CustomerConverter : IMediaConverter<Customer, XElement>
    {
        public Customer FromWireFormat(XElement wireFormat)
        {
            throw new System.NotImplementedException();
        }

        public Customer FromWireFormat(XElement wireFormat, Customer loadThis)
        {
            throw new System.NotImplementedException();
        }

        public XElement ToWireFormat(Customer value)
        {
            return new XElement("Customer",
                                new XElement("Id", value.Id));
        }
    }
}