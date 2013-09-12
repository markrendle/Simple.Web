namespace OwinHostable
{
    using System;
    using System.IO;
    using System.Reflection;

    using Simple.Web.Helpers;

    public sealed class PathUtility : IPathUtility
    {
        public string MapPath(string virtualPath)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetPath());

            if (path == null)
                throw new Exception("Unable to determine executing assembly path.");

            return Path.Combine(path, virtualPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }
    }
}