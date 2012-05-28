namespace Simple.Web.Links
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class LinkAttributeBase : Attribute
    {
        private readonly Type _modelType;
        private readonly string _uriTemplate;

        internal LinkAttributeBase(Type modelType, string uriTemplate)
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

        public abstract string GetRel();
        public string Type { get; set; }
        public string Title { get; set; }

        public static bool Exists(Type type)
        {
            return GetCustomAttributes(type, typeof(LinkAttributeBase)).Length > 0;
        }

        public static IList<LinkAttributeBase> Get(Type type, Type modelType)
        {
            return GetCustomAttributes(type, typeof (LinkAttributeBase))
                .Cast<LinkAttributeBase>()
                .Where(a => a.ModelType == modelType)
                .ToList();
        }
    }
}