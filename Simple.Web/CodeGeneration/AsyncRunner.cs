namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Threading.Tasks;
    using Http;

    /// <summary>
    /// Runs asynchronous handlers. Should only be used from hosting code.
    /// </summary>
    public class AsyncRunner
    {
        private readonly Func<object, IContext, Task<Status>> _start;
        private readonly Action<object, IContext, Status> _end;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRunner"/> class.
        /// </summary>
        /// <param name="start">The start function.</param>
        /// <param name="end">The end action.</param>
        public AsyncRunner(Func<object, IContext, Task<Status>> start, Action<object, IContext, Status> end)
        {
            _start = start;
            _end = end;
        }

        /// <summary>
        /// Gets the end action.
        /// </summary>
        public Action<object, IContext, Status> End
        {
            get { return _end; }
        }

        /// <summary>
        /// Gets the start function.
        /// </summary>
        public Func<object, IContext, Task<Status>> Start
        {
            get { return _start; }
        }
    }
}