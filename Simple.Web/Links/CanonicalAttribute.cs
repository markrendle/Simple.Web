namespace Simple.Web.Links
{
    using System;

    /// <summary>
    /// Apply this attribute to a Handler to denote that it is the canonical URI for a resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CanonicalAttribute : LinkAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalAttribute"/> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="uriTemplate">The URI template.</param>
        /// <remarks>The URI template works in reverse here. The {X} variables will be replaced with the
        /// relevant properties from an instance of the model to create an actual URI to be returned to
        /// the client.</remarks>
        public CanonicalAttribute(Type modelType, string uriTemplate) : base(modelType, uriTemplate)
        {
        }

        /// <summary>
        /// Gets the "rel" attribute for the link; for canonical links, this is always "self".
        /// </summary>
        /// <returns>"self"</returns>
        public override string GetRel()
        {
            return "self";
        }
    }
}