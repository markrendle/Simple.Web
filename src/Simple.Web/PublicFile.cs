namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    using Simple.Web.Cors;
    using Simple.Web.Http;

    public class PublicFile
    {
        private readonly IList<IAccessControlEntry> _accessControl;
        private readonly CacheOptions _cacheOptions;
        private readonly string _path;

        public PublicFile(string path)
            : this(path, (CacheOptions)null)
        {
        }

        public PublicFile(string path, params AccessControlEntry[] accessControl)
            : this(path, null, accessControl)
        {
        }

        public PublicFile(string path, CacheOptions cacheOptions, params AccessControlEntry[] accessControl)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            _path = path;
            _cacheOptions = cacheOptions;
            if (accessControl != null)
            {
                _accessControl = accessControl;
            }
        }

        public IList<IAccessControlEntry> AccessControl
        {
            get { return _accessControl; }
        }

        public CacheOptions CacheOptions
        {
            get { return _cacheOptions; }
        }

        public string Path
        {
            get { return _path; }
        }

        public static implicit operator PublicFile(string path)
        {
            return new PublicFile(path);
        }
    }
}