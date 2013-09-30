namespace Simple.Web.OwinSupport
{
    using System.Collections.Generic;
    using System.IO;

    using Simple.Web.Http;

    internal class OwinContext : IContext
    {
        public OwinContext(IDictionary<string, object> env)
        {
            Variables = env;
            Request = new OwinRequest(env, (IDictionary<string, string[]>)env[OwinKeys.RequestHeaders], (Stream)env[OwinKeys.RequestBody]);
            Response = new OwinResponse();
        }

        public IRequest Request { get; private set; }

        public IResponse Response { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }
    }
}