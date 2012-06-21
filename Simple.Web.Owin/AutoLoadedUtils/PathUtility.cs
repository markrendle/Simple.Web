using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Simple.Web.Helpers;

namespace Simple.Web.Owin.AutoLoadedUtils {
    public sealed class PathUtility : IPathUtility{

		private static readonly string AppRoot = AssemblyAppRoot(Assembly.GetExecutingAssembly().GetPath());

    	public static string AssemblyAppRoot(string typePath)
    	{
    		return Path.GetDirectoryName(typePath).Regex(@"\\bin\\?([Dd]ebug|[Rr]elease)?$", string.Empty);
    	}

    	public string MapPath(string virtualPath) {
			return Path.Combine(AppRoot, virtualPath);
    	}
    }
	
    internal static class RegexEx
    {
        public static string Regex(this string target, string pattern, string replaceWith)
        {
            return new Regex(pattern).Replace(target, replaceWith);
        }
    }
}
