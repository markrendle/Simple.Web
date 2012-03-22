namespace Simple.Web
{
    public class RawHtml
    {
        private readonly string _html;

        internal RawHtml(string html)
        {
            _html = html;
        }

        public string Html
        {
            get { return _html; }
        }

        public override string ToString()
        {
            return _html;
        }

        public static implicit operator RawHtml(string html)
        {
            return new RawHtml(html);
        }
    }
}