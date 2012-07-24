namespace Simple.Web.Behaviors
{
    using System.Threading.Tasks;

    /// <summary>
    /// Adds output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <remarks>For asynchronous handlers, consider using <see cref="IOutputAsync{TOutput}"/></remarks>
    [OutputBehavior(typeof(Implementations.WriteOutput))]
    public interface IOutput<out TOutput>
    {
        /// <summary>
        /// Gets the output.
        /// </summary>
        TOutput Output { get; }
    }

    /// <summary>
    /// Adds asynchronous output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <remarks>Use this interface instead of <see cref="IOutput{T}"/> to preserve an asynchronous pipeline.</remarks>
    public interface IOutputAsync<TOutput>
    {
        /// <summary>
        /// Gets a <see cref="Task{T}"/> which asynchronously returns the output.
        /// </summary>
        Task<TOutput> Output { get; }
    }
}