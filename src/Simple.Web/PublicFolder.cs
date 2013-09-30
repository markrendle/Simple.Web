namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Simple.Web.Cors;
    using Simple.Web.Http;

    public class PublicFolder : IEquatable<PublicFolder>
    {
        private readonly IList<IAccessControlEntry> _accessControl;
        private readonly string _alias;
        private readonly CacheOptions _cacheOptions;
        private readonly string _path;
        private readonly Regex _rewrite;

        public PublicFolder(string path)
            : this(path, path)
        {
        }

        public PublicFolder(string path, params AccessControlEntry[] accessControl)
            : this(path, path, null, accessControl)
        {
        }

        public PublicFolder(string path, CacheOptions cacheOptions)
            : this(path, path, cacheOptions)
        {
        }

        public PublicFolder(string path, string alias)
            : this(path, alias, (CacheOptions)null)
        {
        }

        public PublicFolder(string path, string alias, params AccessControlEntry[] accessControl)
            : this(path, alias, null, accessControl)
        {
        }

        public PublicFolder(string path, string alias, CacheOptions cacheOptions, params AccessControlEntry[] accessControl)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (alias == null)
            {
                throw new ArgumentNullException("alias");
            }
            _path = path;
            _alias = alias;
            _cacheOptions = cacheOptions;
            if (!path.Equals(alias, StringComparison.OrdinalIgnoreCase))
            {
                _rewrite = new Regex("^" + Regex.Escape(alias), RegexOptions.IgnoreCase);
            }
            if (accessControl != null)
            {
                _accessControl = accessControl;
            }
        }

        public IList<IAccessControlEntry> AccessControl
        {
            get { return _accessControl; }
        }

        public string Alias
        {
            get { return _alias; }
        }

        public CacheOptions CacheOptions
        {
            get { return _cacheOptions; }
        }

        public string Path
        {
            get { return _path; }
        }

        public bool Equals(PublicFolder other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(_path, other._path) && string.Equals(_alias, other._alias);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((PublicFolder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_path.GetHashCode()*397) ^ _alias.GetHashCode();
            }
        }

        public string RewriteAliasToPath(string absolutePath)
        {
            return _rewrite == null ? absolutePath : _rewrite.Replace(absolutePath, _path);
        }

        public static bool operator ==(PublicFolder left, PublicFolder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PublicFolder left, PublicFolder right)
        {
            return !Equals(left, right);
        }

        public static implicit operator PublicFolder(string path)
        {
            return new PublicFolder(path);
        }
    }
}