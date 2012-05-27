namespace Simple.Web
{
    public class Link
    {
        private readonly string _href;
        private readonly string _rel;
        private readonly string _type;

        public Link(string href, string rel, string type)
        {
            _href = href;
            _rel = rel;
            _type = type;
        }

        public string Href
        {
            get { return _href; }
        }

        public string Rel
        {
            get { return _rel; }
        }

        public string Type
        {
            get { return _type; }
        }
    }
}