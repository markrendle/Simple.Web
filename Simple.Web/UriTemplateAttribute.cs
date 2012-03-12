using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Web
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class UriTemplateAttribute : Attribute
    {
        private readonly string _template;

        // This is a positional argument
        public UriTemplateAttribute(string template)
        {
            _template = template;
        }

        public string Template
        {
            get { return _template; }
        }

        internal static IEnumerable<UriTemplateAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (UriTemplateAttribute))
                .Cast<UriTemplateAttribute>();
        }
    }
}