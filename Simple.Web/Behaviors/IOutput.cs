namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Adds output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    [OutputBehavior(typeof(Implementations.WriteOutput))]
    public interface IOutput<out TOutput>
    {
        /// <summary>
        /// Gets the output.
        /// </summary>
        TOutput Output { get; }
    }
}