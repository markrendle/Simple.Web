namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class TypeResolver
    {
        public readonly HashSet<Assembly> KnownGoodAssemblies = new HashSet<Assembly>();
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
                if (modelType != null) return modelType;
            }

            if (KnownGoodAssemblies.Any(assembly => (modelType = FindTypeInAssembly(assembly, typeName)) != null))
            {
                return modelType;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if ((modelType = FindTypeInAssembly(assembly, typeName)) != null)
                {
                    KnownGoodAssemblies.Add(assembly);
                    return modelType;
                }
            }

            return null;
        }

        private Type FindTypeInAssembly(Assembly assembly, string typeName)
        {
            var type = assembly.GetType(typeName);
            if (type != null) return type;
            for (int i = 0; i < RazorTypeBuilder.DefaultNamespaceImports.Length; i++)
            {
                type = assembly.GetType(RazorTypeBuilder.DefaultNamespaceImports[i] + "." + typeName);
                if (type != null) return type;
            }
            return null;
        }
    }
}