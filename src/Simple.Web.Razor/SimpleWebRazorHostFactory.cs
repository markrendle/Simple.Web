namespace Simple.Web.Razor
{
    using System.Web.WebPages.Razor;

    public class SimpleWebRazorHostFactory : WebRazorHostFactory
    {
        public override WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            var host = base.CreateHost(virtualPath, physicalPath);

            if (!host.IsSpecialPage)
            {
                return new SimpleWebPageRazorHost(virtualPath, physicalPath);
            }

            return host;
        }
    }
}