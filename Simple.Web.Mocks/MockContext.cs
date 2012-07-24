using System.Collections.Generic;

namespace Simple.Web.Mocks
{
    using System.IO;
    using Authentication;
    using Http;

    public class MockContext : IContext
    {
        private readonly IDictionary<string, object> _variables = new Dictionary<string, object>();

        public MockContext()
        {
            Request = new MockRequest();
            Response = new MockResponse();
        }
        public IRequest Request { get; set; }

        public IResponse Response { get; set; }
        public IDictionary<string, object> Variables
        {
            get { return _variables; }
        }

        public IUser User { get; set; }
    }
}