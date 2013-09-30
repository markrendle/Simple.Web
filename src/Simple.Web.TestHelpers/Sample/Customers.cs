namespace Simple.Web.TestHelpers.Sample
{
    using System.Collections.Generic;

    using Simple.Web.Links;

    [UriTemplate("/customer")]
    public abstract class CustomerRelated
    {
    }

    [UriTemplate("/{Id}")]
    public class Customer : CustomerRelated
    {
        public Customer()
        {
        }

        public Customer(int id)
        {
            Id = id;
        }

        public int Id { get; set; }

        public IList<Order> Orders { get; set; }
    }

    [UriTemplate("/{Id}/contacts")]
    [LinksFrom(typeof(Customer), Rel = "customer.contacts", Type = "application/vnd.contact")]
    public class CustomerContacts : CustomerRelated
    {
    }

    [LinksFrom(typeof(Customer), "/customer/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")]
    public class CustomerOrders
    {
    }

    [Canonical(typeof(Customer), "/customer/{Id}", Type = "application/vnd.customer")]
    public class CustomerHandler
    {
    }

    [LinksFrom(typeof(IEnumerable<Customer>), "/customers", Rel = "self", Type = "application/vnd.list.customer")]
    public class CustomersHandler
    {
    }
}