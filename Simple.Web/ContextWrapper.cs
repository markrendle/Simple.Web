namespace Simple.Web
{
    using System.Web;

    class ContextWrapper : IContext
    {
        private readonly HttpContext _context;
        private readonly RequestWrapper _request;
        private readonly ResponseWrapper _response;

        public ContextWrapper(HttpContext context)
        {
            _context = context;
            _request = new RequestWrapper(context.Request);
            _response = new ResponseWrapper(context.Response);
        }

        public IRequest Request
        {
            get { return _request; }
        }

        public IResponse Response
        {
            get { return _response; }
        }
    }
}