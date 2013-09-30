namespace Simple.Web.TestHelpers
{
    using System;
    using System.IO;
    using System.Text;

    public class StringBuilderStream : Stream
    {
        private readonly MemoryStream _buffer;
        private readonly StreamReader _bufferReader;
        private readonly StringBuilder _resultBuilder;

        public StringBuilderStream()
        {
            _buffer = new MemoryStream();
            _bufferReader = new StreamReader(_buffer, true);
            _resultBuilder = new StringBuilder();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position { get; set; }

        public string StringValue
        {
            get { return _resultBuilder.ToString(); }
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _buffer.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _buffer.EndWrite(asyncResult);
            Flush();
        }

        public override void Flush()
        {
            _buffer.Position = 0;
            _resultBuilder.Append(_bufferReader.ReadToEnd());
            _buffer.SetLength(0);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _buffer.Write(buffer, offset, count);
        }
    }
}