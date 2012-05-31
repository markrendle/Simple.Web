namespace Simple.Web.Http
{
    using System;

    /// <summary>
    /// Represents an HTTP cookie.
    /// </summary>
    public interface ICookie
    {
        /// <summary>
        /// Gets or sets the name of the cookie.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ICookie"/> is secure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if secure; otherwise, <c>false</c>.
        /// </value>
        bool Secure { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the cookie is accessible only by the server.
        /// </summary>
        /// <value>
        ///   <c>true</c> if HTTP only; otherwise, <c>false</c>.
        /// </value>
        bool HttpOnly { get; set; }
        /// <summary>
        /// Gets or sets the time when the cookie expires.
        /// </summary>
        /// <value>
        /// The time when the cookie expires.
        /// </value>
        DateTime Expires { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        string Value { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key in a multi-value cookie.
        /// </summary>
        string this[string key] { get; set; }
    }
}