namespace Simple.Web.AspNet
{
    using System;
    using System.Threading.Tasks;
    using CodeGeneration;
    using Helpers;
    using Hosting;
    using Http;

    class SimpleAsyncHandlerResult : AsyncResult
    {
        private readonly IContext _context;
        private readonly HandlerInfo _handlerInfo;
        private readonly AsyncCallback _callback;
        private readonly ErrorHelper _helper;
        private AsyncRunner _runner;
        private object _handler;

        public SimpleAsyncHandlerResult(IContext context, HandlerInfo handlerInfo, AsyncCallback callback, object asyncState)
        {
            _context = context;
            _handlerInfo = handlerInfo;
            _callback = callback;
            AsyncState = asyncState;
            _helper = new ErrorHelper(context);
        }

        public void Run()
        {
            _handler = HandlerFactory.Instance.GetHandler(_handlerInfo);

            if (_handler != null)
            {
                _runner = HandlerRunnerFactory.Instance.GetAsync(_handlerInfo.HandlerType, _context.Request.HttpMethod);
                _runner.Start(_handler, _context).ContinueWith(RunContinuation);
            }
            else
            {
                throw new InvalidOperationException("Could not create handler handler.");
            }
        }

        private void RunContinuation(Task<Status> t)
        {
            IsCompleted = true;
            if (t.IsFaulted && t.Exception != null)
            {
                _helper.WriteError(t.Exception.InnerException);
            }
            else
            {
                try
                {
                    _runner.End(_handler, _context, t.Result);
                }
                catch (Exception ex)
                {
                    _helper.WriteError(ex);
                }
            }

            _callback(this);
        }
    }
}