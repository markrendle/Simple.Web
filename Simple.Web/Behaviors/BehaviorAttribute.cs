namespace Simple.Web.Behaviors
{
    using System;

    /// <summary>
    /// Base class for <see cref="RequestBehaviorAttribute"/>, <see cref="ResponseBehaviorAttribute"/> and <see cref="OutputBehaviorAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public abstract class BehaviorAttribute : Attribute
    {
        private readonly Type _implementingType;

        protected BehaviorAttribute(Type implementingType)
        {
            _implementingType = implementingType;
        }

        /// <summary>
        /// Gets the type that implements the behaviour.
        /// </summary>
        /// <value>
        /// The implementing type.
        /// </value>
        public Type ImplementingType
        {
            get { return _implementingType; }
        }

        /// <summary>
        /// Gets or sets the priority, allowing control over the order in which behaviors are processed.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public Priority Priority { get; set; }
    }
}