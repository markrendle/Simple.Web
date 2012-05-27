namespace Simple.Web
{
    using System;

    public class Link
    {
        private readonly Type _handlerType;
        private readonly string _href;
        private readonly string _rel;
        private readonly string _type;

        public Link(Type handlerType, string href, string rel, string type)
        {
            _handlerType = handlerType;
            _href = href;
            _rel = rel;
            _type = type;
        }

        public Type HandlerType
        {
            get { return _handlerType; }
        }

        public string Href
        {
            get { return _href; }
        }

        public string Rel
        {
            get { return _rel; }
        }

        public string Type
        {
            get { return _type; }
        }
    }
}