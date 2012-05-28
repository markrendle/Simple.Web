namespace Simple.Web.Links
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class LinksFromAttribute : Attribute
    {
        private readonly Type _modelType;
        private readonly string _uriTemplate;

        public LinksFromAttribute(Type modelType, string uriTemplate)
        {
            _modelType = modelType;
            _uriTemplate = uriTemplate;
        }

        public string UriTemplate
        {
            get { return _uriTemplate; }
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        public string Rel { get; set; }

        public string Type { get; set; }

        public static bool Exists(Type type)
        {
            return GetCustomAttributes(type, typeof(LinksFromAttribute)).Length > 0;
        }

        public static IList<LinksFromAttribute> Get(Type type, Type modelType)
        {
            return GetCustomAttributes(type, typeof (LinksFromAttribute))
                .Cast<LinksFromAttribute>()
                .Where(a => a.ModelType == modelType)
                .ToList();
        }
    }
}