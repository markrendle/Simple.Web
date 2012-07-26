namespace Simple.Web.Owin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Http;

    internal class OwinRequest : IRequest
    {
        private HttpContext _context;

        public OwinRequest(IDictionary<string, object> env, IDictionary<string, string[]> requestHeaders, Stream inputStream)
        {
            HttpMethod = env[OwinKeys.Method].ToString();
            Headers = requestHeaders;
            InputStream = inputStream;
            Url = new Uri(MakeUriString(env, requestHeaders));
            if (env.ContainsKey(OwinKeys.QueryString))
            {
                QueryString = QueryStringParser.Parse((string) env[OwinKeys.QueryString]);
            }
            else
            {
                QueryString = new Dictionary<string, string[]>();
            }
            object aspNetContext;
            if (env.TryGetValue("aspnet.Context", out aspNetContext))
            {
                _context = (HttpContext) aspNetContext;
            }
        }

        public Uri Url { get; private set; }

        public IDictionary<string, string[]> QueryString { get; private set; }

        public Stream InputStream { get; private set; }

        public string HttpMethod { get; private set; }

        public IDictionary<string, string[]> Headers { get; private set; }

        public IEnumerable<IPostedFile> Files
        {
            get
            {
                if (_context != null)
                {
                    for (int i = 0; i < _context.Request.Files.Count; i++)
                    {
                        yield return new PostedFile(_context.Request.Files.Get(i));
                    }
                }
            }
        }

        private static string MakeUriString(IDictionary<string, object> env, IDictionary<string, string[]> requestHeaders)
        {
            var uri = string.Format("{0}://{1}{2}{3}", env[OwinKeys.Scheme], requestHeaders["Host"][0], env[OwinKeys.PathBase], env[OwinKeys.Path]);
            object queryString;
            if (env.TryGetValue(OwinKeys.QueryString, out queryString))
            {
                if (queryString != null && !string.IsNullOrWhiteSpace((string)queryString))
                {
                    uri = uri + "?" + queryString;
                }
            }
            return uri;
        }
    }
}