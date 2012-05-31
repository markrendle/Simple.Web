namespace Simple.Web
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the HTTP Status Code returned by a Handler.
    /// </summary>
    /// <remarks>Has an implicit cast from <see cref="int"/>.</remarks>
    public struct Status : IEquatable<Status>
    {
        /// <summary>
        /// The basic "everything's OK" status.
        /// </summary>
        public static readonly Status OK = new Status(200, "OK");

        /// <summary>
        /// Indicates that a request was processed successfully and a new resource was created.
        /// </summary>
        public static readonly Status Created = new Status(201, "Created");

        /// <summary>
        /// Nothing to see here.
        /// </summary>
        public static readonly Status NoContent = new Status(204, "No Content");

        /// <summary>
        /// A redirect to another resource, commonly used after a POST operation to prevent refreshes.
        /// </summary>
        public static readonly Status SeeOther = new Status(303, "See Other");

        /// <summary>
        /// A Temporary redirect, e.g. for a login page.
        /// </summary>
        public static readonly Status TemporaryRedirect = new Status(307, "Temporary Redirect");

        /// <summary>
        /// Indicates that everything is horrible, and you should hide in a cupboard until it's all over.
        /// </summary>
        public static readonly Status InternalServerError = new Status(500, "Internal Server Error");
        private static readonly StatusLookupCollection StatusLookup;

        static Status()
        {
            StatusLookup = new StatusLookupCollection
                               {
                                   OK,
                                   Created,
                                   NoContent,
                                   SeeOther,
                                   TemporaryRedirect,
                                   InternalServerError,
                               };
        }

        private readonly int _httpStatusCode;
        private readonly string _httpStatusDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Status"/> struct.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public Status(int httpStatusCode) : this(httpStatusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Status"/> struct.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusDescription">The HTTP status description.</param>
        public Status(int httpStatusCode, string httpStatusDescription) : this()
        {
            _httpStatusCode = httpStatusCode;
            _httpStatusDescription = httpStatusDescription;
        }

        public static implicit operator Status(int httpStatus)
        {
            return StatusLookup.Contains(httpStatus) ? StatusLookup[httpStatus] : new Status(httpStatus);
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public int Code
        {
            get { return _httpStatusCode; }
        }

        /// <summary>
        /// Gets the HTTP status description.
        /// </summary>
        public string Description
        {
            get { return _httpStatusDescription; }
        }

        /// <summary>
        /// Gets a value indicating whether this Status represents success.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this Status represents success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess
        {
            get { return _httpStatusCode >= 200 && _httpStatusCode <= 299; }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Status other)
        {
            return _httpStatusCode == other._httpStatusCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Status)) return false;
            return Equals((Status) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
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