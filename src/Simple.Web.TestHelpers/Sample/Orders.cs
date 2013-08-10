namespace Simple.Web.TestHelpers.Sample
{
    using Links;

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

        public int Id { get; set; }
        public int CustomerId { get; set; }
    }

    [Canonical(typeof(Order), "/order/{Id}", Type = "application/vnd.order")]
    public class OrderHandler
    {
    }
}