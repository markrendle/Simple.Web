namespace Simple.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using Xunit;

    public class GetHandlerTests
    {
        [Fact]
        public void GetRootWithHtmlReturnsHtml()
        {
            var target = new GetHandler(typeof(RootEndpoint));
            var context = new MockContext
                              {
                                  Request = new MockRequest
                                                {
                                                    Url = new Uri("http://test.com/"),
                                                    AcceptTypes = new[] {"text/html"},
                                                    HttpMethod = "GET",
                                                }
                              };
            target.HandleRequest(context);
            Assert.Equal(200, context.Response.StatusCode);
            Assert.Equal("text/html", context.Response.ContentType);
            Assert.NotEqual(0, context.Response.OutputStream.Length);
        }
    }

    class MockContext : IContext
    {
        public MockContext()
        {
            Response = new MockResponse
                           {
                               OutputStream = new MockStream()
                           };
        }
        public IRequest Request { get; set; }

        public IResponse Response { get; set; }
    }

    class MockRequest : IRequest
    {
        public MockRequest()
        {
            QueryString = new NameValueCollection();
        }
        public Uri Url { get; set; }

        public IList<string> AcceptTypes { get; set; }

        public NameValueCollection QueryString { get; set; }

        public Stream InputStream { get; set; }

        public string ContentType { get; set; }

        public string HttpMethod { get; set; }
    }

    class MockResponse : IResponse
    {
        private bool _closed;
        private bool _flushed;

        internal bool Flushed
        {
            get { return _flushed; }
        }

        internal bool Closed
        {
            get { return _closed; }
        }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public Stream OutputStream { get; set; }

        public string ContentType { get; set; }

        public void Close()
        {
            _closed = true;
        }

        public void Flush()
        {
            _flushed = true;
        }
    }

    class MockStream : Stream
    {
        private readonly MemoryStream _stream = new MemoryStream();

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public override void Close()
        {
            Position = 0;
        }

        public void ForceClose()
        {
            _stream.Close();
        }
    }

    [UriTemplate("/")]
    [RespondsTo(ContentType.Html)]
    public class RootEndpoint : GetEndpoint<RawHtml>
    {
        protected override Status Get()
        {
            Output = Raw.Html("<h1>Hello</h1>");
            return Status.OK;
        }
    }
}
