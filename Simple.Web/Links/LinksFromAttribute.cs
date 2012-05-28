namespace Simple.Web.Links
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class LinksFromAttribute : LinkAttributeBase
    {
        public LinksFromAttribute(Type modelType, string uriTemplate) : base(modelType, uriTemplate)
        {
        }

        public override string GetRel()
        {
            return Rel;
        }

        public string Rel { get; set; }
    }
}