namespace $rootnamespace$
{
    using Simple.Web;

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