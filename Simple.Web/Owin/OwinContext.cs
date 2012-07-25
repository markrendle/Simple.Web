namespace Simple.Web.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Http;

    internal class OwinContext : IContext
    {
        public OwinContext(IDictionary<string,object> env, IDictionary<string,string[]> requestHeaders, Stream inputStream)
        {
        }

        public IRequest Request { get; private set; }
        public IResponse Response { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }

    internal class OwinRequest : IRequest
    {
        public OwinRequest(IDictionary<string, object> env, IDictionary<string, string[]> requestHeaders, Stream inputStream)
        {
            HttpMethod = env[OwinKeys.Method].ToString();
        }
        public Uri Url { get; private set; }
        public IDictionary<string, string[]> QueryString { get; private set; }
        public Stream InputStream { get; private set; }
        public string HttpMethod { get; private set; }
        public IDictionary<string, string[]> Headers { get; private set; }
        public IEnumerable<IPostedFile> Files { get; private set; }
    }

    internal class OwinResponse : IResponse
    {
        public Status Status { get; set; }
        public Func<Stream, CancellationToken, Task> WriteFunction { get; set; }
        public IDictionary<string, string[]> Headers { get; set; }
    }
}