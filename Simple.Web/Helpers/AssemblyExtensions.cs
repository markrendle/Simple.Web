namespace Simple.Web.Helpers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Helpful extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the local path from an <see cref="AssemblyName"/>.
        /// </summary>
        /// <param name="assemblyName"><see cref="AssemblyName"/> identifying an assembly.</param>
        /// <returns>The local path of the assembly.</returns>
        public static string GetPath(this AssemblyName assemblyName)
        {
            return new Uri(assemblyName.EscapedCodeBase).LocalPath;
        }

        /// <summary>
        /// Gets the local path from an <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">A loaded <see cref="Assembly"/>.</param>
        /// <returns>The local path of the assembly.</returns>
        public static string GetPath(this Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }
    }
}