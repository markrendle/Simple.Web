using System.Collections.Generic;

namespace Simple.Web.AspNet
{
    using System.Web;
    using Authentication;
    using Http;

    class ContextWrapper : IContext
    {
        private readonly HttpContext _context;
        private readonly RequestWrapper _request;
        private readonly ResponseWrapper _response;
        private readonly IUser _user;
        private readonly IDictionary<string, object> _variables = new Dictionary<string,object>();

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

        public IDictionary<string, object> Variables
        {
            get { return _variables; }
        }
    }
}