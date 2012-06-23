using System.Collections.Generic;
using System.IO;

namespace Simple.Web.Owin.FileHandling
{
	public class PostedFile : IPostedFile
    {
        private readonly IDictionary<string, string> _headers; 
        private readonly string _fileName;
        private readonly Stream _stream;

        public PostedFile(string fileName, IDictionary<string,string> headers, Stream stream)
        {
            _fileName = fileName;
            _headers = headers;
            _stream = stream;
        }

		public void SaveAs(string filename)
		{
			throw new System.NotImplementedException();
		}

		public string FileName
        {
            get { return _fileName; }
        }

		public string ContentType
		{
			get { return "???"; } //TODO
		}

		public int ContentLength
		{
			get { return 0;} //TODO
		}

		public Stream InputStream
		{
			get { return _stream; }
		}

		public Stream Stream
        {
            get { return _stream; }
        }

        public IDictionary<string, string> Headers
        {
            get { return _headers; }
        }
    }
}