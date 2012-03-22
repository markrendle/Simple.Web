namespace Simple.Web
{
    using System.Web.Hosting;

    internal sealed class PathUtility : IPathUtility
    {
        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}