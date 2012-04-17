namespace Simple.Web
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class RoutingException : Exception
    {
        private readonly string _url;
        public RoutingException(string url)
        {
            _url = url;
        }

        public RoutingException(string url, string message) : base(message)
        {
            _url = url;
        }

        public RoutingException(string url, string message, Exception inner) : base(message, inner)
        {
            _url = url;
        }

        protected RoutingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            _url = info.GetString("Url");
        }

        public string Url
        {
            get { return _url; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Url", Url);
        }
    }
}