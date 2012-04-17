namespace Simple.Web
{
    using System;
    using System.Threading.Tasks;

    class SimpleAsyncHandlerResult<TEndpointType> : AsyncResult, IDisposable
    {
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly AsyncCallback _callback;
        private readonly HandlerHelper _helper;
        private AsyncEndpointRunner _runner;

        public SimpleAsyncHandlerResult(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider, AsyncCallback callback, object asyncState)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _callback = callback;
            AsyncState = asyncState;
            _helper = new HandlerHelper(context, authenticationProvider);
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
            else
            {
                throw new InvalidOperationException("Could not create endpoint handler.");
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
                _helper.WriteResponse(_runner, t.Result);
            }

            _callback(this);
        }

        public void Dispose()
        {
            DisposeHelper.TryDispose(_runner.Endpoint);
        }
    }
}