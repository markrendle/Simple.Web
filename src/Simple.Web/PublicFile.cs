namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using Cors;
    using Http;

    public class PublicFile
    {
        private readonly string _path;
        private readonly CacheOptions _cacheOptions;
        private readonly IList<IAccessControlEntry> _accessControl;

        public PublicFile(string path) : this(path, (CacheOptions)null)
        {
        }

        public PublicFile(string path, params AccessControlEntry[] accessControl) : this(path, null, accessControl)
        {
        }

        public PublicFile(string path, CacheOptions cacheOptions, params AccessControlEntry[] accessControl)
        {
            if (path == null) throw new ArgumentNullException("path");
            _path = path;
            _cacheOptions = cacheOptions;
            if (accessControl != null)
            {
                _accessControl = accessControl;
            }
        }

        public string Path
        {
            get { return _path; }
        }

        public CacheOptions CacheOptions
        {
            get { return _cacheOptions; }
        }

        public IList<IAccessControlEntry> AccessControl
        {
            get { return _accessControl; }
        }

        public static implicit operator PublicFile(string path)
        {
            return new PublicFile(path);
        }
    }
}