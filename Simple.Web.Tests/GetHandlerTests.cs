namespace Simple.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using Xunit;

    public class GetHandlerTests
    {
        [Fact]
        public void GetRootWithHtmlReturnsHtml()
        {
            var target = new GetHandler(typeof(RootEndpoint));
            var strb = new StringBuilder();
            var context = new MockContext
                              {
                                  Request = new MockRequest
                                                {
                                                    Url = new Uri("http://test.com/"),
                                                    AcceptTypes = new[] { "text/html" },
                                                    HttpMethod = "GET",
                                                },
                                  Response = new MockResponse
                                                 {
                                                     Output = new StringWriter(strb)
                                                 }
                              };
            target.HandleRequest(context);
            Assert.Equal(200, context.Response.StatusCode);
            Assert.Equal("text/html", context.Response.ContentType);
            Assert.NotEqual(0, strb.Length);
        }
    }

    class MockContext : IContext
    {
        public MockContext()
        {
            Response = new MockResponse
                           {
                               OutputStream = new MockStream(),
                               Output = new StringWriter()
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

        public IHeaderCollection Headers { get; set; }
    }

    class MockResponse : IResponse
    {
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public TextWriter Output { get; set; }

        public Stream OutputStream { get; set; }

        public string ContentType { get; set; }

        public IHeaderCollection Headers { get; set; }

        public void Close()
        {
        }

        public void Flush()
        {
        }

        public void Write(string s)
        {
            Write(Encoding.UTF8.GetBytes(s));
        }

        public void Write(object obj)
        {
            if (obj != null) Write(obj.ToString());
        }

        public void Write(char[] buffer, int index, int count)
        {
            Write(Encoding.UTF8.GetBytes(buffer));
        }

        public void Write(char ch)
        {
            Write(Encoding.UTF8.GetBytes(new[] {ch}));
        }

        public void TransmitFile(string file)
        {
        }

        private void Write(byte[] bytes)
        {
            OutputStream.Write(bytes, 0, bytes.Length);
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
    public class RootEndpoint : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            return Status.OK;
        }

        public RawHtml Output { get { return "<h1>Hello</h1>"; } }
    }
}
