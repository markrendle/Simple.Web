namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class HandlerTypeInfo
    {
        private readonly Type _type;
        private readonly HashSet<string> _respondsToAcceptTypes;

        public HandlerTypeInfo(Type type) : this(type, Enumerable.Empty<string>())
        {
        }

        public HandlerTypeInfo(Type type, IEnumerable<string> respondsToAcceptTypes)
        {
            _type = type;
            _respondsToAcceptTypes = new HashSet<string>(respondsToAcceptTypes);
        }

        public Type HandlerType
        {
            get { return _type; }
        }

        public bool RespondsTo(IEnumerable<string> acceptedContentTypes)
        {
            return _respondsToAcceptTypes.Count == 0 || _respondsToAcceptTypes.Overlaps(acceptedContentTypes);
        }
    }
}