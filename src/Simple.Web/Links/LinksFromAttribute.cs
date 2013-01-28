namespace Simple.Web.Links
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Denotes that a handler provides a transition for the specified model type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class LinksFromAttribute : LinkAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinksFromAttribute"/> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="uriTemplate">The URI template.</param>
        public LinksFromAttribute(Type modelType, string uriTemplate = null) : base(modelType, uriTemplate)
        {
        }

        /// <summary>
        /// Gets the rel: the relationship of the linked resource to the current one.
        /// </summary>
        /// <returns>The value of the <see cref="Rel"/> property.</returns>
        /// <remarks>This method is used by Simple.Web</remarks>
        internal override string GetRel()
        {
            return Rel;
        }

        /// <summary>
        /// Gets or sets the rel: the relationship of the linked resource to the current one.
        /// </summary>
        /// <value>
        /// The relationship.
        /// </value>
        /// <remarks>Property exists for setting in Attribute declaration; GetRel is used by the framework.</remarks>
        public string Rel { get; set; }
    }
}