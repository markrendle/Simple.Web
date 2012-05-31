namespace Simple.Web.Mocks
{
    using System.IO;
    using Authentication;
    using Http;

    public class MockContext : IContext
    {
        public MockContext()
        {
            Request = new MockRequest();
            Response = new MockResponse
                           {
                               OutputStream = new MockStream(),
                           };
        }
        public IRequest Request { get; set; }

        public IResponse Response { get; set; }

        public IUser User { get; set; }
    }
}