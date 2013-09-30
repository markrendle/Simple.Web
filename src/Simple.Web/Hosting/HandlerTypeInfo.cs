namespace Simple.Web.Hosting
{
    using System;
    using System.Collections.Generic;

    internal sealed class HandlerTypeInfo
    {
        private readonly int _priority;
        private readonly HashSet<string> _respondsToTypes;
        private readonly HashSet<string> _respondsWithTypes;
        private readonly Type _type;

        public HandlerTypeInfo(Type type)
            : this(type, null, null)
        {
        }

        public HandlerTypeInfo(Type type, IEnumerable<string> respondsToTypes, IEnumerable<string> respondsWithTypes)
        {
            _type = type;
            if (respondsToTypes != null)
            {
                _respondsToTypes = new HashSet<string>(respondsToTypes, StringComparer.OrdinalIgnoreCase);
                if (_respondsToTypes.Count == 0)
                {
                    _respondsToTypes = null;
                }
            }
            if (respondsWithTypes != null)
            {
                _respondsWithTypes = new HashSet<string>(respondsWithTypes, StringComparer.OrdinalIgnoreCase);
                if (_respondsWithTypes.Count == 0)
                {
                    _respondsWithTypes = null;
                }
            }
        }

        private HandlerTypeInfo(Type type, HashSet<string> respondsToTypes, HashSet<string> respondsWithTypes, int priority)
        {
            _type = type;
            _respondsToTypes = respondsToTypes;
            _respondsWithTypes = respondsWithTypes;
            _priority = priority;
        }

        public Type HandlerType
        {
            get { return _type; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public int Property { get; set; }

        public bool RespondsToAll
        {
            get { return _respondsToTypes == null; }
        }

        public bool RespondsWithAll
        {
            get { return _respondsWithTypes == null; }
        }

        public bool RespondsTo(string contentType)
        {
            return _respondsToTypes != null && _respondsToTypes.Contains(contentType);
        }

        public bool RespondsWith(string acceptType)
        {
            return _respondsWithTypes != null && _respondsWithTypes.Contains(acceptType);
        }

        public HandlerTypeInfo SetPriority(int priority)
        {
            return new HandlerTypeInfo(_type, _respondsToTypes, _respondsWithTypes, priority);
        }
    }
}