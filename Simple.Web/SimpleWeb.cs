namespace Simple.Web
{
    public static class SimpleWeb
    {
        public static readonly IConfiguration Configuration = new Configuration();
        public static readonly IWebEnvironment Environment = new WebEnvironment();
    }
}