namespace Sandbox
{
    using Simple.Web;

    [UriTemplate("/hello/{Name}")]
    public class HelloEndpoint : GetEndpoint<RawHtml>
    {
        public string Name { get; set; }
        private string _tag = "h1";
        public string Tag
        {
            get { return _tag; }
            set { _tag = string.IsNullOrWhiteSpace(value) ? "h1" : value; }
        }

        protected override Status Get()
        {
            Output = Raw.Html(string.Format("<{1}>Hello, {0}</{1}>", Name, Tag));
            return Status.OK;
        }
    }
}