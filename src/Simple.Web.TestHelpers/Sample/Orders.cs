namespace Simple.Web.TestHelpers.Sample
{
    using Links;

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
    }

    [Canonical(typeof(Order), "/order/{Id}", Type = "application/vnd.order")]
    public class OrderHandler
    {
    }
}