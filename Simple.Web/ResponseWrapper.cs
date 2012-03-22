namespace Simple.Web
{
    using System.IO;
    using System.Web;

    internal class ResponseWrapper : IResponse
    {
        private readonly HttpResponse _response;
        public void Write(string s)
        {
            _response.Write(s);
        }

        public void Write(object obj)
        {
            _response.Write(obj);
        }

        public void Write(char[] buffer, int index, int count)
        {
            _response.Write(buffer, index, count);
        }

        public ResponseWrapper(HttpResponse response)
        {
            _response = response;
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public string StatusDescription
        {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public TextWriter Output
        {
            get { return _response.Output; }
        }

        public Stream OutputStream
        {
            get { return _response.OutputStream; }
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public void Write(char ch)
        {
            _response.Write(ch);
        }

        public void TransmitFile(string file)
        {
            _response.TransmitFile(file);
        }

        public void End()
        {
            _response.End();
        }

        public void Close()
        {
            _response.Close();
        }

        public void Flush()
        {
            _response.Flush();
        }
    }
}