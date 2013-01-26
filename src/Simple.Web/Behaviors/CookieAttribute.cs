﻿using System;

namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Apply this attribute to a property on a handler class to have it persisted as a cookie.
    /// Primitive properties (including strings and guids) will be persisted as single-value cookies.
    /// Complex properties will be persisted as multi-value cookies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CookieAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the HTTP cookie.
        /// </summary>
        /// <value>
        /// The name of the HTTP cookie. If not set, this will be the same as the property name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds the cookie is valid for.
        /// </summary>
        /// <value>The time out. Set to 0 (the default) for a session-only cookie.</value>
        public int TimeOut { get; set; }

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

        /// <summary>
        /// Gets or sets a value specifying the path below which the cooki
        /// </summary>
        public string Path { get; set; }
    }
}