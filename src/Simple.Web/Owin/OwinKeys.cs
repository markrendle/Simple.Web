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
        public const string RequestHeaders = "owin.RequestHeaders";
        public const string RequestBody = "owin.RequestBody";

        public const string ResponseBody = "owin.ResponseBody";
        public const string ResponseHeaders = "owin.ResponseHeaders";
        public const string StatusCode = "owin.ResponseStatusCode";
        public const string ReasonPhrase = "owin.ResponseReasonPhrase";
        public const string ResponseProtocol = "owin.ResponseProtocol";

        public const string Version = "owin.Version";
        public const string CallCancelled = "owin.CallCancelled";

        public const string ClientCertifiate = "ssl.ClientCertificate";

        public const string ServerRemoteIpAddress = "server.RemoteIpAddress";
        public const string ServerRemotePort = "server.RemotePort";
        public const string ServerLocalIpAddress = "server.LocalIpAddress";
        public const string ServerLocalPort = "server.LocalPort";
        public const string ServerUser = "server.User";
        public const string ServerIsLocal = "server.IsLocal";

        public const string WebSocketSupport = "websocket.Support";
        public const string WebSocketFunc = "websocket.Func";
    }
}