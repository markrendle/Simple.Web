namespace Simple.Web.Razor
{
    using System;
    using System.Reflection;

    static class AssemblyEx
    {
        public static string GetPath(this AssemblyName assemblyName)
        {
            return new Uri(assemblyName.EscapedCodeBase).LocalPath;
        }

        public static string GetPath(this Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }
    }
}