namespace Simple.Web.CodeGeneration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class TestHandler : IGet, IRequireAuthentication, IInput<string>, ISetCookies, ICacheability, IETag, IModified
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
}