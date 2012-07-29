namespace Simple.Web.CodeGeneration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Authentication;
    using Behaviors;
    using Http;

    abstract class TestHandlerBase : IRequireAuthentication, IInput<string>, ICacheability, IETag, IModified
    {
        public IUser CurrentUser { get; set; }

        public string Input { get; set; }

        public CacheOptions CacheOptions
        {
            get { return CacheOptions.DisableCaching; }
        }

        public string InputETag
        {
            set { throw new NotImplementedException(); }
        }

        public string OutputETag
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime? IfModifiedSince
        {
            set { throw new NotImplementedException(); }
        }

        public DateTime? LastModified
        {
            get { throw new NotImplementedException(); }
        }
    }

    class TestHandler : TestHandlerBase, IGet
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
    }
    
    class TestAsyncHandler : TestHandlerBase, IGetAsync
    {
        private readonly Status _status;

        public TestAsyncHandler(Status status)
        {
            _status = status;
        }

        public Task<Status> Get()
        {
            return Task.Factory.StartNew(() => _status);
        }
    }
}