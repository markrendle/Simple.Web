namespace Simple.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    sealed class PublicFileHandler
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

        public bool TryHandleAsFile(Uri uri, IResponse response)
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
    }

    public interface IFileUtility
    {
        bool Exists(string path);
    }
}
