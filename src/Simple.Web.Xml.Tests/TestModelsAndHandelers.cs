using System.Collections.Generic;
using Simple.Web.Links;

namespace Simple.Web.Xml.Tests
{
    [LinksFrom(typeof(Customer), "/customer/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")]
    public class CustomerOrders
    {
    }

    [LinksFrom(typeof(Customer), "/customer/{Id}", Rel = "self", Type = "application/vnd.customer")]
    public class CustomerHandler
    {
    }

    [LinksFrom(typeof(IEnumerable<Customer>), "/customers", Rel = "self", Type = "application/vnd.list.customer")]
    public class CustomersHandler
    {
    }

    public class OrderHandler
    {
    }

    public class Customer
    {
        public int Id { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
    }
}