namespace Simple.Web.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Hosting;

    /// <summary>
    /// Handles routing for hosts.
    /// </summary>
    internal class RoutingTable
    {
        private const int MaximumGroupCount = 64;
        private readonly Dictionary<string, IList<HandlerTypeInfo>> _staticPaths =
            new Dictionary<string, IList<HandlerTypeInfo>>(StringComparer.OrdinalIgnoreCase);

        private readonly List<SortedList<Regex, IList<HandlerTypeInfo>>> _dynamicPaths;

        internal RoutingTable()
        {
            _dynamicPaths = new List<SortedList<Regex, IList<HandlerTypeInfo>>>(GenerateEmptyLists());
        }

        /// <summary>
        /// Gets the type of handler for the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="variables">The values of the variables from the URI template.</param>
        /// <returns></returns>
        public Type Get(string url, out IDictionary<string,string[]> variables)
        {
            variables = null;
            var types = GetTypesForStatic(url) ??
                GetTypesForDynamic(url, out variables);

            return types == null ? null : types.Single().HandlerType;
        }

        /// <summary>
        /// Gets the type of handler for the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="contentType">Value of the Content-Type header from the Request.</param>
        /// <param name="acceptTypes">Values of the Accepts header from the Request.</param>
        /// <param name="variables">The variables.</param>
        /// <returns></returns>
        public Type Get(string url, string contentType, IList<string> acceptTypes, out IDictionary<string, string[]> variables)
        {
            variables = null;
            var types = GetTypesForStatic(url) ??
                GetTypesForDynamic(url, out variables);

            if (types == null) return null;
            var typeInfo = types.SingleOrDefault(t => t.RespondsTo(contentType) && t.RespondsWith(acceptTypes));
            if (typeInfo == null) return null;
            return typeInfo.HandlerType;
        }

        private IEnumerable<HandlerTypeInfo> GetTypesForStatic(string url)
        {
            if (_staticPaths.ContainsKey(url))
            {
                return _staticPaths[url];
            }
            return null;
        }

        private IEnumerable<HandlerTypeInfo> GetTypesForDynamic(string url, out IDictionary<string, string[]> variables)
        {
            for (int i = 0; i < MaximumGroupCount; i++)
            {
                if (_dynamicPaths[i].Count == 0) continue;

                var entry = _dynamicPaths[i].FirstOrDefault(t => t.Key.IsMatch(url));
                if (entry.Key == null)
                {
                    continue;
                }

                variables = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                var match = entry.Key.Match(url);
                foreach (var groupName in entry.Key.GetGroupNames())
                {
                    if (groupName == "0") continue;
                    variables.Add(groupName, new[] {match.Groups[groupName].Value});
                }
                return entry.Value;
            }
            variables = null;
            return null;
        }

        internal void Add(string uriTemplate, Type handlerType)
        {
            Add(uriTemplate, new HandlerTypeInfo(handlerType));
        }

        internal void Add(string uriTemplate, HandlerTypeInfo handlerType)
        {
            if (uriTemplate.Contains("{"))
            {
                var regex = new Regex("^" + Regex.Replace(uriTemplate, "{([^}]*)}", "(?<$1>[^/]*)") + "$", RegexOptions.IgnoreCase);
                _dynamicPaths[regex.GetGroupNames().Length].Add(regex, new[] {handlerType});
            }
            else
            {
                _staticPaths.Add(uriTemplate, new[] {handlerType});
                if (uriTemplate.EndsWith("/"))
                {
                    _staticPaths.Add(uriTemplate.TrimEnd('/'), new[] {handlerType});
                }
                else
                {
                    _staticPaths.Add(uriTemplate + '/', new[] {handlerType});
                }
            }
        }

        internal IEnumerable<Type> GetAllTypes()
        {
            return _staticPaths.Values.SelectMany(list => list.Select(eti => eti.HandlerType))
                .Concat(_dynamicPaths.SelectMany(l => l.Values.SelectMany(t => t)).Select(e => e.HandlerType))
                .Distinct();
        }

        private static IEnumerable<SortedList<Regex, IList<HandlerTypeInfo>>> GenerateEmptyLists()
        {
            var regexTermComparer =
                new Helpers.Comparer<Regex>(
                    (regex, regex1) => StringComparer.OrdinalIgnoreCase.Compare(regex.ToString(), regex1.ToString()));
            for (int i = 0; i < MaximumGroupCount; i++)
            {
                yield return new SortedList<Regex, IList<HandlerTypeInfo>>(regexTermComparer);
            }
        }
    }
}
