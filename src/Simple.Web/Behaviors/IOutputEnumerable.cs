using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.Web.Behaviors
{
    /// <summary>
    /// Adds output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <remarks>For asynchronous handlers, consider using <see cref="IOutputAsync{TOutput}"/></remarks>
    [OutputBehavior(typeof(Implementations.WriteOutputEnumerable))]
    public interface IOutputEnumerable<out TOutput>
    {
        IEnumerable<TOutput> Output { get; }
    }

    /// <summary>
    /// Adds asynchronous output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <remarks>Use this interface instead of <see cref="IOutput{T}"/> to preserve an asynchronous pipeline.</remarks>
    [OutputBehavior(typeof(Implementations.WriteOutputEnumerableAsync))]
    public interface IOutputEnumerableAsync<TOutput>
    {
        /// <summary>
        /// Gets a <see cref="Task"/> which asynchronously returns the output.
        /// </summary>
        Task<IEnumerable<TOutput>> Output { get; }
    }

}