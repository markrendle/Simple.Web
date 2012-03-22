using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using System.Text.RegularExpressions;

    internal class RoutingTable
    {
        private readonly Dictionary<string,IList<EndpointTypeInfo>> _staticPaths = new Dictionary<string, IList<EndpointTypeInfo>>();
        private readonly SortedDictionary<Regex, IList<EndpointTypeInfo>> _dynamicPaths = new SortedDictionary<Regex, IList<EndpointTypeInfo>>(new Comparer<Regex>((x,y) => x.GetGroupNames().Length.CompareTo(y.GetGroupNames().Length)));

        public Type Get(string url, out IDictionary<string,string> variables)
        {
            var types = GetTypesForStatic(url, out variables) ??
                GetTypesForDynamic(url, out variables);

            return types == null ? null : types.Single().EndpointType;
        }

        private IEnumerable<EndpointTypeInfo> GetTypesForDynamic(string url, out IDictionary<string, string> variables)
        {
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

        public Type Get(string url, IList<string> acceptTypes, out IDictionary<string, string> variables)
        {
            var types = GetTypesForStatic(url, out variables) ??
                GetTypesForDynamic(url, out variables);

            if (types == null) return null;
            var typeInfo = types.SingleOrDefault(t => t.RespondsTo(acceptTypes));
            if (typeInfo == null) return null;
            return typeInfo.EndpointType;
        }

        private IEnumerable<EndpointTypeInfo> GetTypesForStatic(string url, out IDictionary<string, string> variables)
        {
            variables = null;
            if (_staticPaths.ContainsKey(url))
            {
                return _staticPaths[url];
            }
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
                var regex = new Regex(Regex.Replace(uriTemplate, "{([^}]*)}", "(?<$1>[^/]*)"), RegexOptions.IgnoreCase);
                _dynamicPaths.Add(regex, new[] {endpointType});
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
                .Concat(_dynamicPaths.Values.SelectMany(list => list.Select(eti => eti.EndpointType)))
                .Distinct();
        }
    }
}
