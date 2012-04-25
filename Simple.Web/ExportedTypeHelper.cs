namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static class ExportedTypeHelper
    {
        public static IEnumerable<Type> FromCurrentAppDomain(Func<Type,bool> predicate)
        {
            var list = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(assembly =>
                        assembly.GetExportedTypes().Where(predicate).ToList())
                .SelectMany(exportedTypes => exportedTypes)
                .ToList();

            return list;
        }
    }
}