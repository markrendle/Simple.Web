namespace Sandbox
{
    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/hello/{Name}")]
    public class GetHello : IGet, IOutput<RawHtml>
    {
        private string _tag = "h1";

        public string Name { get; set; }

        public RawHtml Output
        {
            get { return Raw.Html(string.Format("<{1}>Hello, {0}</{1}>", Name, Tag)); }
        }

        public string OutputETag
        {
            get { return null; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = string.IsNullOrWhiteSpace(value) ? "h1" : value; }
        }

        public Status Get()
        {
            return Status.OK;
        }
    }
}