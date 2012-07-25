namespace Simple.Web.Owin
{
    internal static class OwinKeys
    {
        public const string Method = "owin.RequestMethod";
        public const string Path = "owin.RequestPath";
        public const string PathBase = "owin.RequestPathBase";
        public const string Protocol = "owin.RequestProtocol";
        public const string QueryString = "owin.RequestQueryString";
        public const string Scheme = "owin.RequestScheme";
        public const string Version = "owin.Version";
    }
}