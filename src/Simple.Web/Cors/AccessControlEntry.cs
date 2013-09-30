namespace Simple.Web.Cors
{
    using System.Collections.Generic;

    public class AccessControlEntry : IAccessControlEntry
    {
        private static readonly IEqualityComparer<IAccessControlEntry> OriginComparerInstance = new OriginEqualityComparer();
        private readonly string _allowHeaders;

        private readonly bool? _credentials;
        private readonly string _exposeHeaders;
        private readonly long? _maxAge;
        private readonly string _methods;
        private readonly string _origin;

        public AccessControlEntry(string origin,
                                  string methods = null,
                                  long? maxAge = null,
                                  string allowHeaders = null,
                                  bool? credentials = null,
                                  string exposeHeaders = null)
        {
            _origin = origin;
            _methods = methods;
            _maxAge = maxAge;
            _allowHeaders = allowHeaders;
            _credentials = credentials;
            _exposeHeaders = exposeHeaders;
        }

        public static IEqualityComparer<IAccessControlEntry> OriginComparer
        {
            get { return OriginComparerInstance; }
        }

        public string AllowHeaders
        {
            get { return _allowHeaders; }
        }

        public bool? Credentials
        {
            get { return _credentials; }
        }

        public string ExposeHeaders
        {
            get { return _exposeHeaders; }
        }

        public long? MaxAge
        {
            get { return _maxAge; }
        }

        public string Methods
        {
            get { return _methods; }
        }

        public string Origin
        {
            get { return _origin; }
        }

        private sealed class OriginEqualityComparer : IEqualityComparer<IAccessControlEntry>
        {
            public bool Equals(IAccessControlEntry x, IAccessControlEntry y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (ReferenceEquals(x, null))
                {
                    return false;
                }
                if (ReferenceEquals(y, null))
                {
                    return false;
                }
                if (x.GetType() != y.GetType())
                {
                    return false;
                }
                return string.Equals(x.Origin, y.Origin);
            }

            public int GetHashCode(IAccessControlEntry obj)
            {
                return obj.Origin.GetHashCode();
            }
        }
    }
}