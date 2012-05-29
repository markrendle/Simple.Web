namespace Simple.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExportedTypeHelper
    {
        public static IEnumerable<Type> FromCurrentAppDomain(Func<Type,bool> predicate)
        {
            var list = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !(a.IsDynamic || a.GlobalAssemblyCache)) // Don't want dynamic assemblies, and if they're in the GAC they're probably BCL.
                .Select(assembly =>
                        assembly.GetExportedTypes().Where(predicate).ToList())
                .SelectMany(exportedTypes => exportedTypes)
                .ToList();

            return list;
        }
    }
}