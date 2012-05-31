namespace Simple.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// For working with type reflection at runtime.
    /// </summary>
    public static class ExportedTypeHelper
    {
        /// <summary>
        /// Gets the exported types from all assemblies currently loaded, except for those which are dynamically generated or in the GAC.
        /// </summary>
        /// <param name="predicate">A predicate to filter the types.</param>
        /// <returns>A list of types.</returns>
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