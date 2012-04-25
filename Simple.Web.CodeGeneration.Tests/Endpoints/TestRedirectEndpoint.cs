namespace Simple.Web.CodeGeneration.Tests.Endpoints
{
    using System;
    using System.IO;

    class TestRedirectEndpoint : IGet, IMayRedirect, IOutputStream
    {
        private readonly Status _status;

        public TestRedirectEndpoint(Status status)
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