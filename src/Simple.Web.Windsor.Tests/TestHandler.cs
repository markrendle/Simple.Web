using System;

namespace Simple.Web.Windsor.Tests
{
    public class TestHandler : IGet, IDisposable
    {
        readonly IResult _result;
        public bool IsDisposed { get; set; }

        public TestHandler(IResult result)
        {
            _result = result;
        }

        public Status Get()
        {
            return _result.Result;
        }

        public string TestProperty { get; set; }
        
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}