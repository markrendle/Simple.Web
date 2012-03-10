namespace Simple.Web
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContentTypesAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        // This is a positional argument
        public ContentTypesAttribute(params string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        public string[] ContentTypes
        {
            get { return _contentTypes; }
        }
    }
}