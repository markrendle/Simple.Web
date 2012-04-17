using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using System.Text.RegularExpressions;

    internal class RoutingTable
    {
        private const int MaximumGroupCount = 64;
        private readonly Dictionary<string, IList<EndpointTypeInfo>> _staticPaths =
            new Dictionary<string, IList<EndpointTypeInfo>>(StringComparer.OrdinalIgnoreCase);

        private readonly List<SortedList<Regex, IList<EndpointTypeInfo>>> _dynamicPaths;

        public RoutingTable()
        {
            _dynamicPaths = new List<SortedList<Regex, IList<EndpointTypeInfo>>>(GenerateEmptyLists());
        }

        public Type Get(string url, out IDictionary<string,string> variables)
        {
            variables = null;
            var types = GetTypesForStatic(url) ??
                GetTypesForDynamic(url, out variables);

            return types == null ? null : types.Single().EndpointType;
        }

        public Type Get(string url, IList<string> acceptTypes, out IDictionary<string, string> variables)
        {
            variables = null;
            var types = GetTypesForStatic(url) ??
                GetTypesForDynamic(url, out variables);

            if (types == null) return null;
            var typeInfo = types.SingleOrDefault(t => t.RespondsTo(acceptTypes));
            if (typeInfo == null) return null;
            return typeInfo.EndpointType;
        }

        private IEnumerable<EndpointTypeInfo> GetTypesForStatic(string url)
        {
            if (_staticPaths.ContainsKey(url))
            {
                return _staticPaths[url];
            }
            return null;
        }

        private IEnumerable<EndpointTypeInfo> GetTypesForDynamic(string url, out IDictionary<string, string> variables)
        {
            for (int i = 0; i < MaximumGroupCount; i++)
            {
                if (_dynamicPaths[i].Count == 0) continue;

                var entry = _dynamicPaths[i].FirstOrDefault(t => t.Key.IsMatch(url));
                if (entry.Key == null)
                {
                    continue;
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
            variables = null;
            return null;
        }

        public void Add(string uriTemplate, Type endpointType)
        {
            Add(uriTemplate, new EndpointTypeInfo(endpointType));
        }

        public void Add(string uriTemplate, EndpointTypeInfo endpointType)
        {
            if (uriTemplate.Contains("{"))
            {
                var regex = new Regex("^" + Regex.Replace(uriTemplate, "{([^}]*)}", "(?<$1>[^/]*)") + "$", RegexOptions.IgnoreCase);
                _dynamicPaths[regex.GetGroupNames().Length].Add(regex, new[] {endpointType});
            }
            else
            {
                _staticPaths.Add(uriTemplate, new[] {endpointType});
                if (uriTemplate.EndsWith("/"))
                {
                    _staticPaths.Add(uriTemplate.TrimEnd('/'), new[] {endpointType});
                }
                else
                {
                    _staticPaths.Add(uriTemplate + '/', new[] {endpointType});
                }
            }
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return _staticPaths.Values.SelectMany(list => list.Select(eti => eti.EndpointType))
                .Concat(_dynamicPaths.SelectMany(l => l.Values.SelectMany(t => t)).Select(e => e.EndpointType))
                .Distinct();
        }

        private static IEnumerable<SortedList<Regex, IList<EndpointTypeInfo>>> GenerateEmptyLists()
        {
            var regexTermComparer =
                new Comparer<Regex>(
                    (regex, regex1) => StringComparer.OrdinalIgnoreCase.Compare(regex.ToString(), regex1.ToString()));
            for (int i = 0; i < MaximumGroupCount; i++)
            {
                yield return new SortedList<Regex, IList<EndpointTypeInfo>>(regexTermComparer);
            }
        }
    }
}
