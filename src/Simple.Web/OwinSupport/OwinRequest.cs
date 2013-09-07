namespace Simple.Web.OwinSupport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    using Simple.Web.Http;

    internal class OwinRequest : IRequest
    {
        private readonly HttpContext _context;

        public OwinRequest(IDictionary<string, object> env, IDictionary<string, string[]> requestHeaders, Stream inputStream)
        {
            this.HttpMethod = env[OwinKeys.Method].ToString();
            this.Headers = requestHeaders;
            this.InputStream = inputStream;
            this.Url = new Uri(MakeUriString(env, requestHeaders));
            if (env.ContainsKey(OwinKeys.QueryString))
            {
                this.QueryString = QueryStringParser.Parse((string) env[OwinKeys.QueryString]);
            }
            else
            {
                this.QueryString = new Dictionary<string, string[]>();
            }
            object aspNetContext;
            if (env.TryGetValue("aspnet.Context", out aspNetContext))
            {
                this._context = (HttpContext) aspNetContext;
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
                if (this._context != null)
                {
                    for (int i = 0; i < this._context.Request.Files.Count; i++)
                    {
                        yield return new PostedFile(this._context.Request.Files.Get(i));
                    }
                }
            }
        }

        public string Host
        {
            get { return this.Headers[HeaderKeys.Host][0]; }
        }

        private static string MakeUriString(IDictionary<string, object> env, IDictionary<string, string[]> requestHeaders)
        {
            string[] hostHeaders;
            string host = "localhost";
            if (requestHeaders.TryGetValue("Host", out hostHeaders) && hostHeaders.Length > 0)
            {
                host = hostHeaders[0];
            }
            if (string.IsNullOrWhiteSpace(host)) host = "localhost";
            var scheme = env.GetValueOrDefault(OwinKeys.Scheme, "http");
            var pathBase = env.GetValueOrDefault(OwinKeys.PathBase, string.Empty);
            var path = env.GetValueOrDefault(OwinKeys.Path, "/");
            var uri = string.Format("{0}://{1}{2}{3}", scheme, host, pathBase, path);
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

    internal static class DictionaryEx
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}