using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Mocks
{
    using System.Collections.Specialized;
    using System.IO;

    public class MockContext : IContext
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

        public IUser User { get; set; }
    }

    public class MockRequest : IRequest
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

        public IEnumerable<IPostedFile> Files
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class MockResponse : IResponse
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
            Write(Encoding.UTF8.GetBytes(new[] { ch }));
        }

        public void TransmitFile(string file)
        {
        }

        public void SetCookie(ICookie cookie)
        {
            throw new NotImplementedException();
        }

        public void DisableCache()
        {
            throw new NotImplementedException();
        }

        private void Write(byte[] bytes)
        {
            OutputStream.Write(bytes, 0, bytes.Length);
        }
    }

    public class MockStream : Stream
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
}
