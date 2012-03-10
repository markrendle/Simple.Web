namespace Simple.Web
{
    using System;

    public class UnsupportedMediaTypeException : Exception
    {
        private readonly string[] _contentTypes;

        public UnsupportedMediaTypeException(string contentType)
        {
            _contentTypes = new[] {contentType};
        }
        public UnsupportedMediaTypeException(string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        public string[] ContentTypes
        {
            get { return _contentTypes; }
        }
    }
}