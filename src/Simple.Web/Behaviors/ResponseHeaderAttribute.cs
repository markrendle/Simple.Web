namespace Simple.Web.Behaviors
{
    using System;

    /// <summary>
    /// Apply this attribute to a property on a handler class to have it persisted as a cookie.
    /// Primitive properties (including strings and guids) will be persisted as single-value cookies.
    /// Complex properties will be persisted as multi-value cookies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ResponseHeaderAttribute : Attribute
    {
        private readonly string _fieldName;

        public ResponseHeaderAttribute()
        {
            
        }

        public ResponseHeaderAttribute(string fieldName)
        {
            _fieldName = fieldName;
        }

        /// <summary>
        /// Gets the HTTP header field name.
        /// </summary>
        /// <value>
        /// The HTTP header field name. If not set, this will be the same as the property name.
        /// </value>
        public string FieldName { get { return _fieldName; } }
    }
}