namespace Simple.Web.Behaviors
{
    using System;

    /// <summary>
    /// Should be applied to behavior interfaces that work in the Response phase of a request/response cycle.
    /// </summary>
    /// <remarks>The Response phase occurs after a handler has been run, before the Output phase.</remarks>
    public sealed class ResponseBehaviorAttribute : BehaviorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseBehaviorAttribute"/> class.
        /// </summary>
        /// <param name="implementingType">The type that implements the behavior for the interface.</param>
        public ResponseBehaviorAttribute(Type implementingType)
            : base(implementingType)
        {
        }

        /// <summary>
        /// Gets the <see cref="ResponseBehaviorAttribute"/> for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ResponseBehaviorAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof(ResponseBehaviorAttribute)) as ResponseBehaviorAttribute;
        }
    }
}