namespace Simple.Web
{
    using System;
    using Http;

    public class PublicFile
    {
        private readonly string _path;
        private readonly CacheOptions _cacheOptions;

        public PublicFile(string path) : this(path, null)
        {
        }

        public PublicFile(string path, CacheOptions cacheOptions)
        {
            if (path == null) throw new ArgumentNullException("path");
            _path = path;
            _cacheOptions = cacheOptions;
        }

        public string Path
        {
            get { return _path; }
        }

        public CacheOptions CacheOptions
        {
            get { return _cacheOptions; }
        }

        public static implicit operator PublicFile(string path)
        {
            return new PublicFile(path);
        }
    }
}