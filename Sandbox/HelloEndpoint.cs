namespace Sandbox
{
    using Simple.Web;

    public class HelloEndpoint : GetEndpoint
    {
        protected override string UriTemplate
        {
            get { return "/hello/{Name}"; }
        }

        public string Name { get; set; }
        private string _tag;
        public string Tag
        {
            get { return _tag; }
            set { _tag = string.IsNullOrWhiteSpace(value) ? "h1" : value; }
        }

        protected override object Run()
        {
            return string.Format("<{1}>Hello, {0}</{1}>", Name, Tag);
        }
    }
}