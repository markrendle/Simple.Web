namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Specifies which Content-Types a handler can handle in the Request body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RespondsToAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespondsToAttribute"/> class.
        /// </summary>
        /// <param name="contentTypes">The types which the handler accepts.</param>
        public RespondsToAttribute(params string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        /// <summary>
        /// Gets the acceptable Content types.
        /// </summary>
        public ReadOnlyCollection<string> ContentTypes
        {
            get { return Array.AsReadOnly(_contentTypes); }
        }

        internal static IEnumerable<RespondsToAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (RespondsToAttribute))
                .Cast<RespondsToAttribute>();
        }
    }
}