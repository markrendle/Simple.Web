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
                file = ValidateFile(uri.AbsolutePath);
                if (file == null) return false;
            }

            try
            {
                response.StatusCode = 200;
                response.TransmitFile(file);
                return true;
            }
            catch (FileNotFoundException)
            {
                _knownStaticFiles.TryRemove(uri.AbsolutePath, out file);
            }
            return false;
        }

        private string ValidateFile(string absolutePath)
        {
            string file;
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
                return null;
            }
            if (_environment.FileUtility.Exists(file))
            {
                _knownStaticFiles.TryAdd(absolutePath, file);
            }
            return file;
        }
    }

    public interface IFileUtility
    {
        bool Exists(string path);
    }
}
