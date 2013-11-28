namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Behaviors.Implementations;
    using Cors;
    using Helpers;
    using Http;

    public class StaticContent
    {
        public static bool TryHandleAsStaticContent(IContext context)
        {
            var absolutePath = context.Request.Url.AbsolutePath;
            string file;
            CacheOptions cacheOptions;
            IList<IAccessControlEntry> accessControl;
            if (SimpleWeb.Configuration.PublicFileMappings.ContainsKey(absolutePath))
            {
                var publicFile = SimpleWeb.Configuration.PublicFileMappings[absolutePath];
                file = SimpleWeb.Environment.PathUtility.MapPath(publicFile.Path);
                cacheOptions = publicFile.CacheOptions;
                accessControl = publicFile.AccessControl;
            }
            else if (SimpleWeb.Configuration.AuthenticatedFileMappings.ContainsKey(absolutePath))
            {
                var user = SimpleWeb.Configuration.AuthenticationProvider.GetLoggedInUser(context);
                if (user == null || !user.IsAuthenticated)
                {
                    CheckAuthentication.Redirect(context);
                    return true;
                }
                var publicFile = SimpleWeb.Configuration.AuthenticatedFileMappings[absolutePath];
                file = SimpleWeb.Environment.PathUtility.MapPath(publicFile.Path);
                cacheOptions = publicFile.CacheOptions;
                accessControl = publicFile.AccessControl;
            }
            else
            {
                var folder = SimpleWeb.Configuration.PublicFolders.FirstOrDefault(
                    f => absolutePath.StartsWith(f.Alias + "/", StringComparison.OrdinalIgnoreCase));
                if (folder != null)
                {
                    file = SimpleWeb.Environment.PathUtility.MapPath(folder.RewriteAliasToPath(absolutePath));
                    cacheOptions = folder.CacheOptions;
                    accessControl = folder.AccessControl;
                }
                else
                {
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return false;

            context.Response.Status = Status.OK;
            context.Response.SetContentType(GetContentType(file, context.Request.GetAccept()));
            var fileInfo = new FileInfo(file);
            context.Response.SetContentLength(fileInfo.Length);
            context.Response.SetLastModified(fileInfo.LastWriteTimeUtc);
            if (cacheOptions != null)
            {
                context.Response.SetCacheOptions(cacheOptions);
            }
            if (accessControl != null)
            {
                context.SetAccessControlHeaders(accessControl);
            }
            context.Response.WriteFunction = (stream) =>
            {
                using (var fileStream = File.OpenRead(file))
                {
                    fileStream.CopyTo(stream);
                    return TaskHelper.Completed();
                }
            };

            return true;
        }

        internal static string GetContentType(string file, IEnumerable<string> acceptTypes)
        {
            if (acceptTypes == null) return "text/plain";

            var types = acceptTypes.ToArray();

            if (types.All(r => r == "*/*")) return GuessType(file);
            return types.FirstOrDefault() ?? "text/plain";
        }

        private static string GuessType(string file)
        {
            switch (file.ToLower().SubstringAfterLast('.'))
            {
                case "js":
                case "javascript": return "text/javascript";

                case "css": return "text/css";

                case "jpg":
                case "jpeg": return "image/jpeg";
                case "png": return "image/png";
                case "gif": return "image/gif";

                case "html":
                case "htm":
                case "xhtml": return "text/html";

                default: return "text/plain";
            }
        }
    }
}