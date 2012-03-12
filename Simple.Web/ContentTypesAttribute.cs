namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContentTypesAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        public ContentTypesAttribute(params string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        public string[] ContentTypes
        {
            get { return _contentTypes; }
        }

        public static IEnumerable<ContentTypesAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (ContentTypesAttribute)).Cast<ContentTypesAttribute>();
        }
    }

    public class ContentType
    {
        public const string Json = "application/json";
        public const string Html = "text/html";
        public const string XHtml = "application/xhtml+xml";
        public const string Xml = "application/xml";
    }
}