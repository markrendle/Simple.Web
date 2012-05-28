namespace Simple.Web.Links
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CanonicalAttribute : LinkAttributeBase
    {
        public CanonicalAttribute(Type modelType, string uriTemplate) : base(modelType, uriTemplate)
        {
        }

        public override string GetRel()
        {
            return "self";
        }
    }
}