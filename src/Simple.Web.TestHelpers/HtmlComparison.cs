namespace Simple.Web.TestHelpers
{
    public static class HtmlComparison
    {
        public static string Cleanse(string result)
        {
            return result.Trim().Replace("\n", "").Replace("\r", "");
        }
    }
}