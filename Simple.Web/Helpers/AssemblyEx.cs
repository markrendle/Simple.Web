namespace Simple.Web.Helpers
{
    using System;
    using System.Reflection;

    public static class AssemblyEx
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