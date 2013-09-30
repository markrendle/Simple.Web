namespace Simple.Web.Helpers
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper methods for creating completed or faulted Tasks.
    /// </summary>
    public class TaskHelper
    {
        /// <summary>
        /// Creates a completed <see cref="Task"/> with the specified <c>Result</c> value.
        /// </summary>
        /// <typeparam name="T">The type of the <c>Result</c> value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task{T}"/> set to completed with the specified value as the <c>Result</c>.</returns>
        public static Task<T> Completed<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a completed <see cref="Task"/> with the specified <c>Result</c> value.
        /// </summary>
        /// <returns>A <see cref="Task"/> set to completed with the specified value as the <c>null</c>.</returns>
        public static Task Completed()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a faulted <see cref="Task"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A faulted <see cref="Task"/> with the <c>Exception</c> property set to the specified value.</returns>
        public static Task Exception(Exception exception)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}