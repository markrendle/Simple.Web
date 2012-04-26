namespace Simple.Web
{
    using System;
    using System.Reflection;

    static class AssemblyEx
    {
        public static string GetPath(this Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }
    }
}