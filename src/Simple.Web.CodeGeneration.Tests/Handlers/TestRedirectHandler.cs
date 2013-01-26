namespace Simple.Web.CodeGeneration.Tests.Handlers
{
    using System;
    using System.IO;
    using Behaviors;

    class TestRedirectHandler : IGet, IMayRedirect, IOutputStream
    {
        private readonly Status _status;

        public TestRedirectHandler(Status status)
        {
            _status = status;
        }

        public Status Get()
        {
            return _status;
        }

        public string Location
        {
            get { throw new NotImplementedException(); }
        }

        public Stream Output
        {
            get { throw new NotImplementedException(); }
        }

        public string ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public string ContentDisposition
        {
            get { throw new NotImplementedException(); }
        }
    }
}