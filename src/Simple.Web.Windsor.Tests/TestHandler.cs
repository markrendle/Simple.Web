namespace Simple.Web.Windsor.Tests
{
    using System;

    public class TestHandler : IGet, IDisposable
    {
        private readonly IResult _result;

        public TestHandler(IResult result)
        {
            _result = result;
        }

        public bool IsDisposed { get; set; }

        public string TestProperty { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public Status Get()
        {
            return _result.Result;
        }
    }
}