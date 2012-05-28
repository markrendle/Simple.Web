namespace Simple.Web.Links
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Link
    {
        private readonly Type _handlerType;
        private readonly string _href;
        private readonly string _rel;
        private readonly string _type;
        private readonly string _title;

        public Link(Type handlerType, string href, string rel, string type, string title)
        {
            _handlerType = handlerType;
            _href = href;
            _rel = rel;
            _type = type;
            _title = title;
        }

        [DataMember]
        public string Title
        {
            get { return _title; }
            private set { }
        }

        public Type GetHandlerType()
        {
            return _handlerType;
        }

        [DataMember]
        public string Href
        {
            get { return _href; }
            private set { }
        }

        [DataMember]
        public string Rel
        {
            get { return _rel; }
            private set { }
        }

        [DataMember]
        public string Type
        {
            get { return _type; }
            private set { }
        }
    }
}