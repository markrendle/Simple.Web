namespace Simple.Web.Behaviors
{
    using System;

    /// <summary>
    /// Should be applied to behavior interfaces that work in the Output phase of a request/response cycle.
    /// </summary>
    /// <remarks>The Output phase occurs after the Response phase is complete.
    /// HEAD requests prevent the Output phase from being called.</remarks>
    public sealed class OutputBehaviorAttribute : BehaviorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputBehaviorAttribute"/> class.
        /// </summary>
        /// <param name="implementingType">The type that implements the behavior for the interface.</param>
        public OutputBehaviorAttribute(Type implementingType)
            : base(implementingType)
        {
        }

        /// <summary>
        /// Gets the <see cref="OutputBehaviorAttribute"/> for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static OutputBehaviorAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof(OutputBehaviorAttribute)) as OutputBehaviorAttribute;
        }
    }
}