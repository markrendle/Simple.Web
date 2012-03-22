namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Web;

    [Serializable]
    public sealed class UnsupportedMediaTypeException : HttpException
    {
        private readonly ReadOnlyCollection<string> _contentTypes;

        public UnsupportedMediaTypeException(string contentType) : base(415, "Requested type(s) not available")
        {
            _contentTypes = new ReadOnlyCollection<string>(new[] {contentType});
        }

        public UnsupportedMediaTypeException(IList<string> contentTypes) : base(415, "Requested type(s) not available")
        {
            _contentTypes = new ReadOnlyCollection<string>(contentTypes);
        }

        public ReadOnlyCollection<string> ContentTypes
        {
            get { return _contentTypes; }
        }
    }
}