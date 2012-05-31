namespace Simple.Web.AspNet
{
    using System.Web;
    using Http;

    class ContextWrapper : IContext
    {
        private readonly HttpContext _context;
        private readonly RequestWrapper _request;
        private readonly ResponseWrapper _response;
        private readonly IUser _user;

        public ContextWrapper(HttpContext context)
        {
            _context = context;
            _request = new RequestWrapper(context.Request);
            _response = new ResponseWrapper(context.Response);
            _user = new User(context.User);
        }

        public IUser User
        {
            get { return _user; }
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