namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Indicates that a type is a handler, and specifies the URI template that it matches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class UriTemplateAttribute : Attribute
    {
        private readonly bool _inheritFromBaseClass;
        private readonly string _template;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriTemplateAttribute"/> class.
        /// </summary>
        /// <param name="template">The URI template.</param>
        /// <param name="inheritFromBaseClass">If set to <c>true</c> (the default) any URI template from a base class will be prepended to the specified template.</param>
        public UriTemplateAttribute(string template, bool inheritFromBaseClass = true)
        {
            _template = template;
            _inheritFromBaseClass = inheritFromBaseClass;
        }

        /// <summary>
        /// Gets a value indicating whether the URI template from the base class is inherited.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if start of template is inherited from base class; otherwise, <c>false</c>.
        /// </value>
        public bool InheritFromBaseClass
        {
            get { return _inheritFromBaseClass; }
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        public string Template
        {
            get { return _template; }
        }

        internal static IEnumerable<UriTemplateAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof(UriTemplateAttribute), false).Cast<UriTemplateAttribute>();
        }

        internal static IEnumerable<string> GetAllTemplates(Type type)
        {
            return
                Get(type)
                    .Where(a => !a.InheritFromBaseClass)
                    .Select(a => a.Template)
                    .Concat(RecursivelyBuildTemplates(type, Get(type).Where(a => a.InheritFromBaseClass).Select(a => a.Template)));
        }

        private static bool FindBaseTypeWithUriTemplateAttribute(ref Type type)
        {
            while (type != null && type.BaseType != typeof(object))
            {
                type = type.BaseType;
                if (Get(type).Any())
                {
                    return true;
                }
            }
            return false;
        }

        private static IEnumerable<string> RecursivelyBuildTemplates(Type type, IEnumerable<string> suffixes)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!FindBaseTypeWithUriTemplateAttribute(ref type))
            {
                return suffixes;
            }

            return from prefix in GetAllTemplates(type) from suffix in suffixes select prefix.TrimEnd('/') + '/' + suffix.TrimStart('/');
        }
    }
}