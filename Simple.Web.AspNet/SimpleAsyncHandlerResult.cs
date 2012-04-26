namespace Simple.Web.AspNet
{
    using System;
    using System.Threading.Tasks;
    using CodeGeneration;
    using Helpers;

    class SimpleAsyncHandlerResult : AsyncResult
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly AsyncCallback _callback;
        private readonly ErrorHelper _helper;
        private AsyncRunner _runner;
        private object _endpoint;

        public SimpleAsyncHandlerResult(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider, AsyncCallback callback, object asyncState)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _callback = callback;
            AsyncState = asyncState;
            _helper = new ErrorHelper(context);
        }

        public void Run()
        {
            _endpoint = EndpointFactory.Instance.GetEndpoint(_endpointInfo);

            if (_endpoint != null)
            {
                _runner = EndpointRunnerFactory.Instance.GetAsync(_endpointInfo.EndpointType);
                _runner.Start(_endpoint, _context).ContinueWith(RunContinuation);
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
                try
                {
                    _runner.End(_endpoint, _context, t.Result);
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