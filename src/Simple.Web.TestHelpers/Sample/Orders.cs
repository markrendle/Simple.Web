namespace Simple.Web.TestHelpers.Sample
{
    using Simple.Web.Links;

    public class Order
    {
        public Order()
        {
        }

        public Order(int customerId, int id)
        {
            CustomerId = customerId;
            Id = id;
        }

        public int CustomerId { get; set; }

        public int Id { get; set; }
    }

    [Canonical(typeof(Order), "/order/{Id}", Type = "application/vnd.order")]
    public class OrderHandler
    {
    }
}