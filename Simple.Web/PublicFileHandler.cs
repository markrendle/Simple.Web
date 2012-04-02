namespace Simple.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Web;

    sealed class PublicFileHandler : IHttpHandler
    {
        private readonly ConcurrentDictionary<string,string> _knownStaticFiles = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly IConfiguration _configuration;
        private readonly IWebEnvironment _environment;

        public PublicFileHandler() : this(SimpleWeb.Configuration, SimpleWeb.Environment)
        {
        }

        public PublicFileHandler(IConfiguration configuration, IWebEnvironment environment)
        {
            _configuration = configuration ?? SimpleWeb.Configuration;
            _environment = environment ?? SimpleWeb.Environment;
        }

        public bool TryHandleAsFile(Uri uri, HttpResponse response)
        {
            string file;
            if (!_knownStaticFiles.TryGetValue(uri.AbsolutePath, out file))
            {
                if (!TryMapFilePath(uri.AbsolutePath, out file)) return false;
            }

            try
            {
                response.StatusCode = 200;
                response.TransmitFile(file);
                _knownStaticFiles.TryAdd(uri.AbsolutePath, file);
                return true;
            }
            catch (FileNotFoundException)
            {
                _knownStaticFiles.TryRemove(uri.AbsolutePath, out file);
            }
            return false;
        }

        private bool TryMapFilePath(string absolutePath, out string file)
        {
            if (_configuration.PublicFileMappings.ContainsKey(absolutePath))
            {
                file = _environment.PathUtility.MapPath(_configuration.PublicFileMappings[absolutePath]);
            }
            else if (
                _configuration.PublicFolders.Any(
                    folder => absolutePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase)))
            {
                file = _environment.PathUtility.MapPath(absolutePath);
            }
            else
            {
                file = null;
                return false;
            }

            if (!File.Exists(file)) return false;
            return true;
        }

        public static bool IsPublicFile(string absolutePath, IConfiguration configuration)
        {
            return configuration.PublicFileMappings.ContainsKey(absolutePath)
                   ||
                   configuration.PublicFolders.Any(
                       folder => absolutePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase));
        }

        public void ProcessRequest(HttpContext context)
        {
            string file;
            if (TryMapFilePath(context.Request.Url.AbsolutePath, out file) && File.Exists(file))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = GetContentType(file, context.Request.AcceptTypes);
                context.Response.TransmitFile(file);
                return;
            }

            context.Response.StatusCode = 404;
        }

        private static string GetContentType(string file, string[] acceptTypes)
        {
            if (acceptTypes == null) return "text/text";
            return acceptTypes.FirstOrDefault() ?? "text/text";
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    public interface IFileUtility
    {
        bool Exists(string path);
    }
}
