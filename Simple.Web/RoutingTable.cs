using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using System.Text.RegularExpressions;

    internal class RoutingTable
    {
        private readonly Dictionary<string,Type> _staticPaths = new Dictionary<string, Type>();
        private readonly SortedDictionary<Regex, Type> _dynamicPaths = new SortedDictionary<Regex, Type>(new Comparer<Regex>((x,y) => x.GetGroupNames().Length.CompareTo(y.GetGroupNames().Length)));
 
        public Type Get(string url, out IDictionary<string,string> variables)
        {
            if (_staticPaths.ContainsKey(url))
            {
                variables = null;
                return _staticPaths[url];
            }

            var entry = _dynamicPaths.FirstOrDefault(t => t.Key.IsMatch(url));
            if (entry.Key == null)
            {
                variables = null;
                return null;
            }

            variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var match = entry.Key.Match(url);
            foreach (var groupName in entry.Key.GetGroupNames())
            {
                if (groupName == "0") continue;
                variables.Add(groupName, match.Groups[groupName].Value);
            }
            return entry.Value;
        }

        public void Add(string uriTemplate, Type endpointType)
        {
            if (uriTemplate.Contains("{"))
            {
                var regex = new Regex(Regex.Replace(uriTemplate, "{([^}]*)}", "(?<$1>[^/]*)"), RegexOptions.IgnoreCase);
                _dynamicPaths.Add(regex, endpointType);
            }
            else
            {
                _staticPaths.Add(uriTemplate, endpointType);
                if (uriTemplate.EndsWith("/"))
                {
                    _staticPaths.Add(uriTemplate.TrimEnd('/'), endpointType);
                }
                else
                {
                    _staticPaths.Add(uriTemplate + '/', endpointType);
                }
            }
        }
    }

    class Comparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _compare; 
        public Comparer(Func<T, T, int> compare)
        {
            if (compare == null) throw new ArgumentNullException("compare");
            _compare = compare;
        }

        public int Compare(T x, T y)
        {
            return _compare(x, y);
        }
    }
}
