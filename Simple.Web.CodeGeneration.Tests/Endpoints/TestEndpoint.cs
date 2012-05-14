namespace Simple.Web.CodeGeneration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;

    class TestHandler : IGet, IRequireAuthentication, IInput<string>, ISetCookies, INoCache
    {
        private readonly Status _status;

        public TestHandler(Status status)
        {
            _status = status;
        }

        public Status Get()
        {
            return _status;
        }

        public IUser CurrentUser { get; set; }

        public string Input { get; set; }

        public ICookieCollection ResponseCookies { get; set; }
    }
}