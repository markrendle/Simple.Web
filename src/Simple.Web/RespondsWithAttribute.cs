namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Denotes which Content-Types a handler may serve.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RespondsWithAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespondsWithAttribute"/> class.
        /// </summary>
        /// <param name="contentTypes">The Content-Types served by the handler.</param>
        public RespondsWithAttribute(params string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        /// <summary>
        /// Gets the Content-Types served by the handler.
        /// </summary>
        public ReadOnlyCollection<string> ContentTypes
        {
            get { return Array.AsReadOnly(_contentTypes); }
        }

        internal static IEnumerable<RespondsWithAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (RespondsWithAttribute))
                .Cast<RespondsWithAttribute>();
        }
    }
}