namespace Simple.Web.AspNet
{
    using System.Web.Hosting;
    using Helpers;

    public sealed class PathUtility : IPathUtility
    {
        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}