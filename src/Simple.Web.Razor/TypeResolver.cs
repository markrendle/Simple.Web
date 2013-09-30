namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class TypeResolver
    {
        internal static readonly IEnumerable<Assembly> DefaultAssemblies =
            SimpleRazorConfiguration.NamespaceImports.Where(ni => ni.Value != null)
                                    .Select(ni => ni.Value)
                                    .Where(an => !an.IsDynamic)
                                    .GroupBy(an => an.Location)
                                    .Select(an => an.First())
                                    .ToArray();

        internal static readonly IEnumerable<Assembly> KnownAssemblies =
            AppDomain.CurrentDomain.GetAssemblies()
                     .Where(
                         an =>
                         !an.IsDynamic &&
                         !DefaultAssemblies.Any(da => an.Location.Equals(da.Location, StringComparison.InvariantCultureIgnoreCase)))
                     .ToArray();

        private readonly HashSet<Assembly> _knownGoodAssemblies = new HashSet<Assembly>();
        private readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

        public Type FindType(string typeName)
        {
            return _typeCache.GetOrAdd(typeName, FindTypeImpl);
        }

        private Type FindTypeImpl(string typeName)
        {
            if (typeName.Contains("<"))
            {
                typeName = TypeNameTranslator.CSharpNameToTypeName(typeName);
            }

            return Type.GetType(typeName, null, ResolveType);
        }

        private Type FindTypeInAssembly(Assembly assembly, string typeName)
        {
            var type = assembly.GetType(typeName);

            if (type != null)
            {
                return type;
            }

            foreach (var ns in SimpleRazorConfiguration.NamespaceImports.Select(x => x.Key).Distinct())
            {
                type = assembly.GetType(ns + "." + typeName);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private Type ResolveType(Assembly startAssembly, string typeName, bool ignoreCase)
        {
            return _typeCache.GetOrAdd(typeName, t => ResolveTypeImpl(startAssembly, t, ignoreCase));
        }

        private Type ResolveTypeImpl(Assembly startAssembly, string typeName, bool ignoreCase)
        {
            Type modelType = null;

            if (startAssembly != null)
            {
                modelType = startAssembly.GetType(typeName, false, ignoreCase);
                if (modelType != null)
                {
                    return modelType;
                }
            }

            if (_knownGoodAssemblies.Any(assembly => (modelType = FindTypeInAssembly(assembly, typeName)) != null))
            {
                return modelType;
            }

            foreach (var assembly in DefaultAssemblies.Union(KnownAssemblies))
            {
                if ((modelType = FindTypeInAssembly(assembly, typeName)) != null)
                {
                    _knownGoodAssemblies.Add(assembly);
                    return modelType;
                }
            }

            return null;
        }
    }
}