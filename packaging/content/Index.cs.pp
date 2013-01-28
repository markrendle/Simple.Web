namespace $rootnamespace$
{
    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/")]
    public class Index : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            return 200;
        }

        public RawHtml Output
        {
            get { return "<h2>Simple.Hello</h2>"; }
        }
    }
}