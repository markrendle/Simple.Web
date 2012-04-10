using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Web
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class UriTemplateAttribute : Attribute
    {
        private readonly string _template;
        private readonly bool _inheritFromBaseClass;

        // This is a positional argument
        public UriTemplateAttribute(string template, bool inheritFromBaseClass = true)
        {
            _template = template;
            _inheritFromBaseClass = inheritFromBaseClass;
        }

        public bool InheritFromBaseClass
        {
            get { return _inheritFromBaseClass; }
        }

        public string Template
        {
            get { return _template; }
        }

        internal static IEnumerable<UriTemplateAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (UriTemplateAttribute), false)
                .Cast<UriTemplateAttribute>();
        }

        internal static IEnumerable<string> GetAllTemplates(Type type)
        {
            return Get(type).Where(a => !a.InheritFromBaseClass).Select(a => a.Template)
                .Concat(RecursivelyBuildTemplates(type, Get(type).Where(a => a.InheritFromBaseClass).Select(a => a.Template)));
        }

        private static IEnumerable<string> RecursivelyBuildTemplates(Type type, IEnumerable<string> suffixes)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!FindBaseTypeWithUriTemplateAttribute(ref type)) return suffixes;

            return from prefix in GetAllTemplates(type)
                   from suffix in suffixes
                   select prefix.TrimEnd('/') + '/' + suffix.TrimStart('/');
        }

        private static bool FindBaseTypeWithUriTemplateAttribute(ref Type type)
        {
            while (type != null && type.BaseType != typeof (object))
            {
                type = type.BaseType;
                if (Get(type).Any())
                {
                    return true;
                }
            }
            return false;
        }
    }
}