namespace OwinHostable
{
    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/")]
    public class GetIndex : IGet, IOutput<RawHtml>
    {
        public RawHtml Output { get; private set; }

        public Status Get()
        {
            Output = "OwinHost runnnig Simple.Web!";

            return 200;
        }
    }
}