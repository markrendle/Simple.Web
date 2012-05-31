namespace Simple.Web.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class HandlerTypeInfo
    {
        private readonly Type _type;
        private readonly HashSet<string> _respondsWithTypes;
        private readonly HashSet<string> _respondsToTypes;

        public HandlerTypeInfo(Type type) : this(type, null, null)
        {
        }

        public HandlerTypeInfo(Type type, IEnumerable<string> respondsToTypes, IEnumerable<string> respondsWithTypes)
        {
            _type = type;
            _respondsWithTypes = new HashSet<string>(respondsWithTypes ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            _respondsToTypes = new HashSet<string>(respondsToTypes ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        }

        public Type HandlerType
        {
            get { return _type; }
        }

        public bool RespondsWith(IEnumerable<string> contentTypes)
        {
            return _respondsWithTypes.Count == 0 || _respondsWithTypes.Overlaps(contentTypes);
        }

        public bool RespondsTo(string contentType)
        {
            return _respondsToTypes.Count == 0 || _respondsToTypes.Contains(contentType);
        }
    }
}