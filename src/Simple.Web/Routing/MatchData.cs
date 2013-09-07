using System;
using System.Collections.Generic;
using System.Linq;
using Simple.Web.Hosting;

namespace Simple.Web.Routing
{
    class MatchData
    {
        private bool _set;
        private HandlerTypeInfo _single;
        private List<HandlerTypeInfo> _list;
        private HandlerTypeInfo[] _prioritised;
        private IDictionary<string, string> _variables;

        public IDictionary<string, string> Variables
        {
            get { return _variables; }
        }

        public List<HandlerTypeInfo> List
        {
            get { return _list; }
        }

        private IEnumerable<HandlerTypeInfo> PrioritiseList()
        {
            return _prioritised ?? (_prioritised = _list.OrderBy(hti => hti.Priority).ToArray());
        }

        public HandlerTypeInfo Single
        {
            get { return _single; }
        }

        public void Add(IList<HandlerTypeInfo> typeInfos)
        {
            if (!_set)
            {
                if (typeInfos.Count == 1)
                {
                    _single = typeInfos[0];
                }
                else
                {
                    _list = new List<HandlerTypeInfo>(typeInfos);
                }
                _set = true;
            }
            else
            {
                if (_single != null)
                {
                    _list = typeInfos as List<HandlerTypeInfo> ?? typeInfos.ToList();
                    _list.Insert(0, _single);
                    _single = null;
                }
                else
                {
                    _list.AddRange(typeInfos);
                }
            }
        }

        public void SetVariable(string key, string value)
        {
            if (_variables == null)
            {
                _variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            if (_variables.ContainsKey(key))
            {
                // Append this value with a delimiter
                _variables[key] += "\t" + value;
            }
            else
            {
                _variables.Add(key, value);
            }
        }

        public Type ResolveByMediaTypes(string contentType, IList<string> acceptTypes)
        {
            if (contentType == null)
            {
                if (acceptTypes == null)
                {
                    var match = PrioritiseList().FirstOrDefault(hti => hti.RespondsToAll && hti.RespondsWithAll);
                    if (match != null) return match.HandlerType;
                    return PrioritiseList().First().HandlerType;
                }
                return ResolveByAcceptTypes(acceptTypes);
            }
            if (acceptTypes == null)
            {
                return ResolveByContentType(contentType);
            }
            return ResolveByBoth(contentType, acceptTypes);
        }

        private Type ResolveByBoth(string contentType, IList<string> acceptTypes)
        {
            HandlerTypeInfo match;
            foreach (var acceptType in acceptTypes)
            {
                match = PrioritiseList().FirstOrDefault(hti => hti.RespondsTo(contentType) && hti.RespondsWith(acceptType));
                if (match != null) return match.HandlerType;
            }
            foreach (var acceptType in acceptTypes)
            {
                match = PrioritiseList().FirstOrDefault(hti => hti.RespondsToAll && hti.RespondsWith(acceptType));
                if (match != null) return match.HandlerType;
            }
            match = PrioritiseList().FirstOrDefault(hti => hti.RespondsWithAll && hti.RespondsTo(contentType))
                    ?? PrioritiseList().FirstOrDefault(hti => hti.RespondsToAll && hti.RespondsWithAll);
            return match == null ? null : match.HandlerType;
        }

        private Type ResolveByContentType(string contentType)
        {
            var match = PrioritiseList().FirstOrDefault(hti => hti.RespondsTo(contentType))
                        ?? PrioritiseList().FirstOrDefault(hti => hti.RespondsToAll);
            return match == null ? null : match.HandlerType;
        }

        private Type ResolveByAcceptTypes(IEnumerable<string> acceptTypes)
        {
            HandlerTypeInfo match;
            foreach (var acceptType in acceptTypes)
            {
                match = PrioritiseList().FirstOrDefault(hti => hti.RespondsWith(acceptType));
                if (match != null)
                {
                    return match.HandlerType;
                }
            }

            match = PrioritiseList().FirstOrDefault(hti => hti.RespondsWithAll);
            return match == null ? null : match.HandlerType;
        }
    }
}