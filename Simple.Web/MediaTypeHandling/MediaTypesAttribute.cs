namespace Simple.Web.MediaTypeHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Specifies which content types an implementation of <see cref="IMediaTypeHandler"/> is used for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class MediaTypesAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTypesAttribute"/> class.
        /// </summary>
        /// <param name="contentTypes">The content types.</param>
        public MediaTypesAttribute(params string[] contentTypes)
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
        /// Gets a collection of <see cref="MediaTypesAttribute"/> instances for a specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<MediaTypesAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (MediaTypesAttribute)).Cast<MediaTypesAttribute>();
        }
    }
}