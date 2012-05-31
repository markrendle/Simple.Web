namespace Simple.Web.ContentTypeHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Specifies which content types an implementation of <see cref="IContentTypeHandler"/> is used for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContentTypesAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypesAttribute"/> class.
        /// </summary>
        /// <param name="contentTypes">The content types.</param>
        public ContentTypesAttribute(params string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        /// <summary>
        /// Gets the content types.
        /// </summary>
        public string[] ContentTypes
        {
            get { return _contentTypes; }
        }

        /// <summary>
        /// Gets a collection of <see cref="ContentTypesAttribute"/> instances for a specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<ContentTypesAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (ContentTypesAttribute)).Cast<ContentTypesAttribute>();
        }
    }
}