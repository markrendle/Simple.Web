namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Threading.Tasks;
    using Http;

    public class AsyncRunner
    {
        private readonly Func<object, IContext, Task<Status>> _start;
        private readonly Action<object, IContext, Status> _end;

        public AsyncRunner(Func<object, IContext, Task<Status>> start, Action<object, IContext, Status> end)
        {
            _start = start;
            _end = end;
        }

        public Action<object, IContext, Status> End
        {
            get { return _end; }
        }

        public Func<object, IContext, Task<Status>> Start
        {
            get { return _start; }
        }
    }
}