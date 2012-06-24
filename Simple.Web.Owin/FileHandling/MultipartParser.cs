using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Simple.Web.Owin.FileHandling
{
	public class MultipartParser
    {
        private const int MaximumMemoryStreamSize = 1024*64;

        private static readonly HashSet<byte> EndOfLine = new HashSet<byte>(new[] { LF, CR }); 
        private readonly Stream _source;
        private const byte LF = (byte)'\n';
        private const byte CR = (byte) '\r';
        private readonly byte[] _boundary;
        private readonly int _boundaryLength;
        private readonly byte[] _check;
        private int _byte = -1;
        private Stream _buffer = Stream.Null;
        private bool _atEndOfFile;

        public MultipartParser(Stream source, string boundary)
        {
            _source = source;
            if (source == null) throw new ArgumentNullException("source");
            if (boundary == null) throw new ArgumentNullException("boundary");
            if (!source.CanRead) throw new ArgumentException("Stream specificed by source must be readable.");
            _boundary = Encoding.UTF8.GetBytes("--" + boundary);
            _boundaryLength = _boundary.Length;
            _check = new byte[_boundaryLength + 2];
            _check[0] = _boundary[0];
        }
        
        public IEnumerable<PostedFile> Parse()
        {
            if (_source.Length == 0) yield break;

            if (!MovePastNextBoundary()) yield break;

            while (!_atEndOfFile)
            {
                PostedFile file = GetNextFile();
                if (file == null) break;
                yield return file;
            }
        }

        private PostedFile GetNextFile()
        {
            string fileName;
            while (!TryGetFileName(out fileName))
            {
                if (!MovePastNextBoundary())
                {
                    return null;
                }
            }

            var headers = GetHeaders();
            _buffer = new MemoryStream();
            if (MovePastNextBoundary())
            {
                _buffer.Position = 0;
                return new PostedFile(fileName, headers, _buffer);
            }
            return null;
        }

        private bool MovePastNextBoundary()
        {
            int b, cr = 0, lf = 0;
            bool atStartOfLine = true;

            while ((b = _source.ReadByte()) > -1)
            {
                // if b is CR or LF, hold onto it
                if (b == CR)
                {
                    cr = BufferNewlineChar(cr, b, CR);
                    atStartOfLine = true;
                }
                else if (b == LF)
                {
                    lf = BufferNewlineChar(lf, b, LF);
                    atStartOfLine = true;
                }
                else if (atStartOfLine)
                {
                    atStartOfLine = false;
                    if (b == _boundary[0])
                    {
                        int checkIndex;
                        for (checkIndex = 1; checkIndex < _boundaryLength; checkIndex++)
                        {
                            b = _source.ReadByte();
                            if (b < 0) return false;
                            _check[checkIndex] = (byte)b;
                            if (b != _boundary[checkIndex])
                            {
                                break;
                            }
                        }
                        if (checkIndex == _boundaryLength)
                        {
                            _check[checkIndex++] = (byte)_source.ReadByte();
                            _check[checkIndex++] = (byte)_source.ReadByte();
                            if (AtBoundary())
                            {
                                return true;
                            }
                            WriteBufferBytes(_check, 0, checkIndex);
                        }
                        else
                        {
                            WriteBufferBytes(_check, 0, checkIndex + 1);
                        }
                    }
                    else
                    {
                        if (cr > 0) WriteBufferByte(CR);
                        if (lf > 0) WriteBufferByte(LF);
                        WriteBufferByte((byte)b);
                        cr = lf = 0;
                    }
                }
                else
                {
                    if (cr > 0) WriteBufferByte(CR);
                    if (lf > 0) WriteBufferByte(LF);
                    WriteBufferByte((byte)b);
                    cr = lf = 0;
                }
            }

            _atEndOfFile = true;
            return false;
        }

        // WriteBufferByte and WriteBufferBytes could change MemoryStream to FileStream
        private void WriteBufferByte(byte b)
        {
            _buffer.WriteByte(b);
            if (_buffer.Length > MaximumMemoryStreamSize)
            {
                SwitchToTempFileStream();
            }
        }

        private void WriteBufferBytes(byte[] b, int offset, int count)
        {
            _buffer.Write(b, offset, count);
            if (_buffer.Length > MaximumMemoryStreamSize)
            {
                SwitchToTempFileStream();
            }
        }

        private void SwitchToTempFileStream()
        {
            var tempFileStream = TempFileStream.New();
            _buffer.CopyTo(tempFileStream);
            _buffer.Dispose();
            _buffer = tempFileStream;
        }

        private int BufferNewlineChar(int bx, int b, byte newlineChar)
        {
            if (bx == newlineChar)
            {
                _buffer.WriteByte(newlineChar);
            }
            else
            {
                bx = b;
            }
            return bx;
        }

        private bool AtBoundary()
        {
            for (int i = 0; i < _boundaryLength; i++)
            {
                if (_check[i] != _boundary[i])
                {
                    return false;
                }
            }
            return true;
        }

        private IDictionary<string, string> GetHeaders()
        {
            byte[] chunk;
            var headers = new Dictionary<string, string>();
            while (!EndOfLine.Contains((chunk = GetLineBytes())[0]))
            {
                var line = Encoding.UTF8.GetString(chunk);
                var pair = GetPair(line);
                if (pair.Key != null)
                {
                    headers.Add(pair.Key, pair.Value);
                }
            }

            return headers;
        }

        private bool TryGetFileName(out string fileName)
        {
            fileName = null;
            var chunk = GetLineBytes();
            if (chunk == null) return false;
            var line = Encoding.UTF8.GetString(chunk);
            if (!line.StartsWith("Content-Disposition")) return false;

            if (!TryGetFileName(line, out fileName)) return false;
            return true;
        }

        public static bool TryGetFileName(string line, out string fileName)
        {
            fileName = null;
            const string start = "; filename=\"";
            const int offset = 12;
            int filenameIndex = line.IndexOf(start, StringComparison.OrdinalIgnoreCase);
            if (filenameIndex < 0) return false;

            line = line.Substring(filenameIndex + offset);
            int quoteIndex = line.IndexOf('"');
            if (quoteIndex < 0) throw new InvalidOperationException("Unterminated filename in multipart content.");
            fileName = line.Substring(0, quoteIndex);
            return true;
        }

        public static KeyValuePair<string,string> GetPair(string line)
        {
            int colon = line.IndexOf(':');
            if (colon < 0) return new KeyValuePair<string, string>(null, null);
            return new KeyValuePair<string, string>(line.Substring(0, colon), line.Substring(colon + 1).Trim());
        }

        private byte[] GetLineBytes()
        {
            if (_byte == -1)
            {
                _byte = _source.ReadByte();
            }
            if (_byte < 0) return null;
            using (var buffer = new MemoryStream())
            {
                while (_byte >= 0 && _byte != LF && _byte != CR)
                {
                    buffer.WriteByte((byte)_byte);
                    _byte = _source.ReadByte();
                }

                if (_byte > 0)
                {
                    buffer.WriteByte((byte) _byte);

                    if (_byte == CR)
                    {
                        _byte = _source.ReadByte();
                        if (_byte == LF)
                        {
                            buffer.WriteByte(LF);
                            _byte = -1;
                        }
                    }
                }
                return buffer.GetBuffer();
            }
        }
    }
}
