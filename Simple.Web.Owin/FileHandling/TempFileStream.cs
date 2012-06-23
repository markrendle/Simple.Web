using System;
using System.IO;

namespace Simple.Web.Owin.FileHandling
{
	internal class TempFileStream : Stream
    {
        private readonly string _file;
        private Stream _fileStream;

        public override void Close()
        {
            _fileStream.Close();
        }

        public override void Flush()
        {
            _fileStream.Flush();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _fileStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _fileStream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _fileStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _fileStream.EndWrite(asyncResult);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _fileStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _fileStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _fileStream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return _fileStream.ReadByte();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _fileStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            _fileStream.WriteByte(value);
        }

        public override bool CanRead
        {
            get { return _fileStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _fileStream.CanSeek; }
        }

        public override bool CanTimeout
        {
            get { return _fileStream.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return _fileStream.CanWrite; }
        }

        public override long Length
        {
            get { return _fileStream.Length; }
        }

        public override long Position
        {
            get { return _fileStream.Position; }
            set { _fileStream.Position = value; }
        }

        public override int ReadTimeout
        {
            get { return _fileStream.ReadTimeout; }
            set { _fileStream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return _fileStream.WriteTimeout; }
            set { _fileStream.WriteTimeout = value; }
        }

        private TempFileStream(string file, Stream fileStream)
        {
            _file = file;
            _fileStream = fileStream;
        }

        public static TempFileStream New()
        {
            var file = Path.GetTempFileName();
            var fileStream = System.IO.File.Open(file, FileMode.Open, FileAccess.ReadWrite);
            return new TempFileStream(file, fileStream);
        }

        protected override void Dispose(bool disposing)
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }
            if (System.IO.File.Exists(_file))
            {
                try
                {
                    System.IO.File.Delete(_file);
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }
    }
}