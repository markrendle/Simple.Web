using System;
using System.Collections.Generic;
using Simple.Web.Hosting;

namespace Simple.Web.Routing
{
    abstract class MatcherBase : IMatcher
    {
        private readonly int _priority;
        private readonly string _pattern;
        private readonly MatcherCollection _matchers = new MatcherCollection();
        private readonly Dictionary<string, IMatcher> _statics = new Dictionary<string, IMatcher>(StringComparer.OrdinalIgnoreCase);
        private List<HandlerTypeInfo> _typeInfos;
        private int _totalPriority;

        protected MatcherBase(string pattern, int priority)
        {
            _pattern = pattern;
            _priority = priority;
        }

        public void AddTypeInfo(HandlerTypeInfo typeInfo)
        {
            if (_typeInfos == null)
            {
                _typeInfos = new List<HandlerTypeInfo>();
            }
            _typeInfos.Add(typeInfo.SetPriority(_totalPriority));
        }

        public IList<HandlerTypeInfo> Items { get { return _typeInfos; } }

        public string Pattern { get { return _pattern; } }

        public MatcherCollection Matchers { get { return _matchers; } }

        public bool Match(string part, string value, int index, MatchData matchData)
        {
            if (!OnMatch(part, matchData))
            {
                return false;
            }
            if (index == -1)
            {
                if (_typeInfos == null) return false;
                matchData.Add(_typeInfos);
                return true;
            }
            int nextIndex = value.IndexOf('/', ++index);
            part = nextIndex == -1 ? value.Substring(index) : value.Substring(index, nextIndex - index);
            IMatcher matcher;
            if (_statics.TryGetValue(part, out matcher))
            {
                return matcher.Match(part, value, nextIndex, matchData);
            }
            bool found = false;
            foreach (var t in _matchers)
            {
                found = t.Match(part, value, nextIndex, matchData) || found;
            }
            return found;
        }

        protected abstract bool OnMatch(string part, MatchData matchData);

        public IMatcher Add(string[] parts, int index, int priority)
        {
            _totalPriority = priority + _priority;
            if (index >= parts.Length) return this;
            IMatcher matcher;
            if (!_statics.TryGetValue(parts[index], out matcher))
            {
                if (_matchers.Contains(parts[index]))
                {
                    matcher = _matchers[parts[index]];
                }
                else
                {
                    matcher = MatcherFactory.Create(parts[index]);
                    if (matcher is StaticMatcher)
                    {
                        _statics.Add(parts[index], matcher);
                    }
                    else
                    {
                        _matchers.Add(matcher);
                    }
                }
            }
            return matcher.Add(parts, index + 1, _totalPriority);
        }

        internal IEnumerable<IMatcher> StaticMatchers
        {
            get { return _statics.Values; }
        }
    }
}