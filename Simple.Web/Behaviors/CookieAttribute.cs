using System;

namespace Simple.Web.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CookieAttribute : Attribute
    {
        private TimeSpan _timeOut = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the name of the HTTP cookie.
        /// </summary>
        /// <value>
        /// The name of the HTTP cookie. If not set, this will be the same as the property name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the time the cookie is valid for.
        /// </summary>
        /// <value>The time out. Set to <see cref="TimeSpan.Zero"/> (the default) for a session-only cookie.</value>
        public TimeSpan TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is HTTP only, that is, cannot be read by client-side script.
        /// </summary>
        /// <value>
        /// <c>true</c> if HTTP only, <c>false</c> if not. Default is <c>false</c>.
        /// </value>
        public bool HttpOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie is sent over HTTPS.
        /// </summary>
        /// <value>
        /// <c>true</c> if secure, <c>false</c> if not. Default is <c>false</c>.
        /// </value>
        public bool Secure { get; set; }
    }
}