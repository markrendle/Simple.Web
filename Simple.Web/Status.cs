namespace Simple.Web
{
    using System;
    using System.Collections.ObjectModel;

    public struct Status : IEquatable<Status>
    {
        public static readonly Status OK = new Status(200, "OK");
        public static readonly Status NoContent = new Status(204, "No Content");
        public static readonly Status InternalServerError = new Status(500, "Internal Server Error");
        private static readonly StatusLookupCollection StatusLookup;

        static Status()
        {
            StatusLookup = new StatusLookupCollection
                               {
                                   OK,
                                   NoContent,
                                   InternalServerError
                               };
        }

        private readonly int _httpStatusCode;
        private readonly string _httpStatusDescription;

        public Status(int httpStatusCode) : this(httpStatusCode, null)
        {
        }

        public Status(int httpStatusCode, string httpStatusDescription) : this()
        {
            _httpStatusCode = httpStatusCode;
            _httpStatusDescription = httpStatusDescription;
        }

        public static implicit operator Status(int httpStatus)
        {
            return StatusLookup.Contains(httpStatus) ? StatusLookup[httpStatus] : new Status(httpStatus);
        }

        public int Code
        {
            get { return _httpStatusCode; }
        }

        public string Description
        {
            get { return _httpStatusDescription; }
        }

        public bool IsSuccess
        {
            get { return _httpStatusCode >= 200 && _httpStatusCode <= 299; }
        }

        public bool Equals(Status other)
        {
            return _httpStatusCode == other._httpStatusCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Status)) return false;
            return Equals((Status) obj);
        }

        public override int GetHashCode()
        {
            return _httpStatusCode;
        }

        public static bool operator ==(Status left, Status right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Status left, Status right)
        {
            return !left.Equals(right);
        }

        private class StatusLookupCollection : KeyedCollection<int,Status>
        {
            protected override int GetKeyForItem(Status item)
            {
                return item.Code;
            }
        }
    }
}