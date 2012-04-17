namespace Simple.Web
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class SimpleAsyncHandlerResult<TEndpointType> : IAsyncResult, IDisposable
    {
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly AsyncCallback _callback;
        private readonly object _asyncState;
        private readonly HandlerHelper _helper;
        private volatile bool _isCompleted;
        private AsyncEndpointRunner _runner;

        public SimpleAsyncHandlerResult(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider, AsyncCallback callback, object asyncState)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _callback = callback;
            _asyncState = asyncState;
            _helper = new HandlerHelper(endpointInfo, context, authenticationProvider);
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public object AsyncState
        {
            get { return _asyncState; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public void Run()
        {
            var endpoint = EndpointFactory.Instance.GetEndpoint(_endpointInfo);

            if (endpoint != null)
            {
                if (!_helper.CheckAuthentication(endpoint))
                {
                    _callback(this);
                    return;
                }
                _helper.SetContext(endpoint);
                _helper.SetFiles(endpoint);
                _runner = AsyncEndpointRunner.Create<TEndpointType>(endpoint);
                _runner.BeforeRun(_context, _contentTypeHandlerTable);
                _runner.Run().ContinueWith(RunContinuation);
            }
        }

        private void RunContinuation(Task<Status> t)
        {
            _isCompleted = true;
            if (t.IsFaulted && t.Exception != null)
            {
                _helper.WriteError(t.Exception.InnerException);
                return;
            }

            _helper.WriteResponse(_runner, t.Result);

            _callback(this);
        }

        public void Dispose()
        {
            var disposable = _runner.Endpoint as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}