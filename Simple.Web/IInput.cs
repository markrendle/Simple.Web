namespace Simple.Web
{
    /// <summary>
    /// Represents a handler that is expecting input.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    public interface IInput<in TInput>
    {
        /// <summary>
        /// Sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        /// <remarks>
        /// This property will be set by the framework.
        /// </remarks>
        TInput Input { set; }
    }
}