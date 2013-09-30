namespace Simple.Web.Behaviors
{
    using System.Threading.Tasks;

    using Simple.Web.Behaviors.Implementations;

    /// <summary>
    /// Adds output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <remarks>For asynchronous handlers, consider using <see cref="IOutputAsync{TOutput}"/></remarks>
    [OutputBehavior(typeof(WriteOutput))]
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
    [OutputBehavior(typeof(WriteOutputAsync))]
    public interface IOutputAsync<TOutput>
    {
        /// <summary>
        /// Gets a <see cref="Task{TResult}"/> which asynchronously returns the output.
        /// </summary>
        Task<TOutput> Output { get; }
    }
}