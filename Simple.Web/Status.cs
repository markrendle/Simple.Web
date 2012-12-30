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
        /// Indicates that a request was processed successfully and a new resource was created.
        /// </summary>
        /// <param name="redirectLocation">The redirect location.</param>
        /// <returns></returns>
        public static Status CreatedRedirect(string redirectLocation)
        {
            return new Status(201, "Created", redirectLocation);
        }

        /// <summary>
        /// Nothing to see here.
        /// </summary>
        public static readonly Status NoContent = new Status(204, "No Content");

        /// <summary>
        /// A redirect to another resource, telling the client to use the new URI for all future requests.
        /// </summary>
        public static Status MovedPermanently(string redirectLocation)
        {
            return new Status(301, "Moved Permanently", redirectLocation);
        }

        /// <summary>
        /// A redirect to another resource, but telling the client to continue to use this URI for future requests.
        /// </summary>
        public static Status Found(string redirectLocation)
        {
            return new Status(302, "Found", redirectLocation);
        }

        /// <summary>
        /// A redirect to another resource, commonly used after a POST operation to prevent refreshes.
        /// </summary>
        public static Status SeeOther(string redirectLocation)
        {
            return new Status(303, "See Other", redirectLocation);
        }

        /// <summary>
        /// A Temporary redirect, e.g. for a login page.
        /// </summary>
        public static Status TemporaryRedirect(string redirectLocation)
        {
            return new Status(307, "Temporary Redirect", redirectLocation);
        }

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
                                   InternalServerError,
                               };
        }

        private readonly int _httpStatusCode;
        private readonly string _httpStatusDescription;
        private readonly string _redirectLocation;

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
        public Status(int httpStatusCode, string httpStatusDescription) : this(httpStatusCode, httpStatusDescription, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Status"/> struct.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="httpStatusDescription">The HTTP status description.</param>
        /// <param name="redirectLocation">Redirection Url</param>
        public Status(int httpStatusCode, string httpStatusDescription, string redirectLocation) : this()
        {
            _httpStatusCode = httpStatusCode;
            _httpStatusDescription = httpStatusDescription;
            _redirectLocation = redirectLocation;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="Simple.Web.Status"/>.
        /// </summary>
        /// <param name="httpStatus">The HTTP status code.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Status(int httpStatus)
        {
            return StatusLookup.Contains(httpStatus) ? StatusLookup[httpStatus] : new Status(httpStatus);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32" /> to <see cref="Simple.Web.Status" />.
        /// </summary>
        /// <param name="source">The string source.</param>
        /// <returns>A <see cref="Status"/> object for the specified status.</returns>
        /// <example>
        /// Status status = 404 + "Not Found";
        /// </example>
        /// <exception cref="System.InvalidCastException"></exception>
        public static implicit operator Status(string source)
        {
            try
            {
                return new Status(int.Parse(source.Substring(0, 3)), source.Substring(3).Trim());
            }
            catch (Exception)
            {
                throw new InvalidCastException("Status can only be implicitly cast from an integer, or a string of the format 'nnnSss...s', e.g. '404Not Found'.");
            }
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

        public string RedirectLocation
        {
            get { return _redirectLocation; }
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

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Status left, Status right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
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

        /// <summary>
        /// Returns an HTTP formatted representation of the <see cref="Status"/>.
        /// </summary>
        /// <returns>
        /// E.g. <c>200 OK</c> or <c>404 Not Found</c>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Code, Description);
        }
    }
}