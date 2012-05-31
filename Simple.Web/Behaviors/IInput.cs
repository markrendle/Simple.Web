namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Represents a handler that is expecting input.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    [RequestBehavior(typeof(Implementations.SetInput))]
    public interface IInput<in TInput>
    {
        /// <summary>
        /// Used by the framework to provide the input model.
        /// </summary>
        /// <value>
        /// The input model.
        /// </value>
        TInput Input { set; }
    }
}