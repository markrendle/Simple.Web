namespace Simple.Web.AspNet
{
    using System;
    using System.Web;
    using Http;

    internal class SimpleHttpAsyncHandler : IHttpAsyncHandler
    {
        private readonly IContext _context;
        private readonly HandlerInfo _handlerInfo;
        private readonly IAuthenticationProvider _authenticationProvider;

        internal SimpleHttpAsyncHandler(IContext context, HandlerInfo handlerInfo) : this(context, handlerInfo, null)
        {
        }

        internal SimpleHttpAsyncHandler(IContext context, HandlerInfo handlerInfo, IAuthenticationProvider authenticationProvider)
        {
            _context = context;
            _handlerInfo = handlerInfo;
            _authenticationProvider = authenticationProvider;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var result = new SimpleAsyncHandlerResult(_context, _handlerInfo, cb, extraData);
            result.Run();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }
    }
}