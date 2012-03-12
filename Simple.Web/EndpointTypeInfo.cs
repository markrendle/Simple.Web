namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class EndpointTypeInfo
    {
        private readonly Type _type;
        private readonly HashSet<string> _respondsToAcceptTypes;

        public EndpointTypeInfo(Type type) : this(type, Enumerable.Empty<string>())
        {
        }

        public EndpointTypeInfo(Type type, IEnumerable<string> respondsToAcceptTypes)
        {
            _type = type;
            _respondsToAcceptTypes = new HashSet<string>(respondsToAcceptTypes);
        }

        public Type EndpointType
        {
            get { return _type; }
        }

        public bool RespondsTo(IList<string> acceptedContentTypes)
        {
            return _respondsToAcceptTypes.Count == 0 || _respondsToAcceptTypes.Overlaps(acceptedContentTypes);
        }
    }
}