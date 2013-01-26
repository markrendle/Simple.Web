namespace Simple.Web
{
    using System.Threading.Tasks;
    using Http;

    /// <summary>
    /// Represents a synchronous handler for a PUT operation.
    /// </summary>
    [HttpMethod("PUT")]
    public interface IPut
    {
        /// <summary>
        /// The entry point for the Handler
        /// </summary>
        /// <returns>A <see cref="Status"/> representing the status of the operation.</returns>
        /// <remarks>You can also return an <see cref="int"/> from this method, as long as it is a valid HTTP Status Code.</remarks>
        Status Put();
    }

    /// <summary>
    /// Represents an asynchronous handler for a PUT operation.
    /// </summary>
    [HttpMethod("PUT")]
    public interface IPutAsync
    {
        /// <summary>
        /// The entry point for the Handler
        /// </summary>
        /// <returns>A <see cref="Task&lt;Status&gt;"/> representing the status of the operation.</returns>
        /// <returns>The returned task should be the final task in a chain of continuations.
        /// This is easiest if you use C# 5 and the async/await pattern, but in C# 4 you can use
        /// Task.ContinueWith to achieve the same result, albeit in a head-hurting way.
        /// When implementing async handlers, ensure that any output is fully materialised before
        /// this task completes; for example, if you have an IEnumerable, call ToList on it. If the Output
        /// property is lazily evaluated, you may still get unwanted blocking behavior when the response is written.</returns>
        Task<Status> Put();
    }

    /// <summary>
    /// Represents a synchronous handler for a PUT operation where the input model is passes as a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the input model</typeparam>
    [HttpMethod("PUT")]
    public interface IPut<in T>
    {
        /// <summary>
        /// The entry point for the Handler.
        /// </summary>
        /// <param name="input">The input model, deserialized from the Request stream.</param>
        /// <returns>A <see cref="Status"/> representing the status of the operation.</returns>
        /// <remarks>You can also return an <see cref="int"/> from this method, as long as it is a valid HTTP Status Code.</remarks>
        Status Put(T input);
    }

    /// <summary>
    /// Represents an asynchronous handler for a PUT operation.
    /// </summary>
    /// <typeparam name="T">The type of the input model</typeparam>
    [HttpMethod("PUT")]
    public interface IPutAsync<in T>
    {
        /// <summary>
        /// The entry point for the Handler
        /// </summary>
        /// <param name="input">The input model, deserialized from the Request stream.</param>
        /// <returns>A <see cref="Task&lt;Status&gt;"/> representing the status of the operation.</returns>
        /// <returns>The returned task should be the final task in a chain of continuations.
        /// This is easiest if you use C# 5 and the async/await pattern, but in C# 4 you can use
        /// Task.ContinueWith to achieve the same result, albeit in a head-hurting way.
        /// When implementing async handlers, ensure that any output is fully materialised before
        /// this task completes; for example, if you have an IEnumerable, call ToList on it. If the Output
        /// property is lazily evaluated, you may still get unwanted blocking behavior when the response is written.</returns>
        Task<Status> Put(T input);
    }
}