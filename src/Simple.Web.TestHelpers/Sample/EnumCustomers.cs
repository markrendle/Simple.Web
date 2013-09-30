namespace Simple.Web.TestHelpers.Sample
{
    using Simple.Web.Links;

    public enum MyEnum
    {
        Ian,
        Franc
    }

    public class EnumCustomer
    {
        public MyEnum AnEnum { get; set; }
    }

    [LinksFrom(typeof(EnumCustomer), "/enumcustomer/{Id}", Rel = "self", Type = "application/vnd.enum.customer")]
    public class EnumCustomerHandler
    {
    }
}