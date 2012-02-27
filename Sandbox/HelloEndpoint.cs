namespace Sandbox
{
    using Simple.Web;

    public class HelloEndpoint : GetEndpoint
    {
        public override string UriTemplate
        {
            get { return "/hello/{Name}"; }
        }

        public string Name { get; set; }
        public string Tag { get; set; }

        protected override object Run()
        {
            if (string.IsNullOrWhiteSpace(Tag)) Tag = "h1";
            return string.Format("<{1}>Hello, {0}</{1}>", Name, Tag);
        }
    }
}