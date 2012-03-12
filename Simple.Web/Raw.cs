namespace Simple.Web
{
    using System.Text;

    public static class Raw
    {
        public static RawHtml Html(string html)
        {
            return new RawHtml(html);
        }

        public static RawHtml Html(StringBuilder html)
        {
            return new RawHtml(html.ToString());
        }
    }
}