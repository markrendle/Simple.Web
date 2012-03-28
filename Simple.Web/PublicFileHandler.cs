namespace Simple.Web
{
    using System;
    using System.Linq;
    sealed class PublicFileHandler
    {
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
            if (_configuration.PublicFileMappings.ContainsKey(uri.AbsolutePath))
            {
                file = _environment.PathUtility.MapPath(_configuration.PublicFileMappings[uri.AbsolutePath]);
            }
            else if (_configuration.PublicFolders.Any(folder => uri.AbsolutePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase)))
            {
                file = _environment.PathUtility.MapPath(uri.AbsolutePath);
            }
            else
            {
                return false;
            }

            if (_environment.FileUtility.Exists(file))
            {
                response.StatusCode = 200;
                response.TransmitFile(file);
                return true;
            }

            return false;
        }
    }

    public interface IFileUtility
    {
        bool Exists(string path);
    }
}
