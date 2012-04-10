namespace Simple.Web
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    class SimpleAsyncHandlerResult<TEndpointType> : IAsyncResult, IDisposable
    {
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly AsyncCallback _callback;
        private readonly object _asyncState;
        private volatile bool _isCompleted;
        private AsyncEndpointRunner _runner;

        public SimpleAsyncHandlerResult(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider, AsyncCallback callback, object asyncState)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _authenticationProvider = authenticationProvider;
            _callback = callback;
            _asyncState = asyncState;
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
                if (!CheckAuthentication(endpoint))
                {
                    _callback(this);
                    return;
                }
                SetContext(endpoint);
                _runner = AsyncEndpointRunner.Create<TEndpointType>(endpoint);
                _runner.BeforeRun(_context, _contentTypeHandlerTable);
                RunEndpoint(_runner);
            }
        }

        private bool CheckAuthentication(object endpoint)
        {
            var requireAuthentication = endpoint as IRequireAuthentication;
            if (requireAuthentication == null) return true;

            var user = _authenticationProvider.GetLoggedInUser(_context);
            if (user == null || !user.IsAuthenticated)
            {
                _context.Response.StatusCode = 401;
                _context.Response.StatusDescription = "Unauthorized";
                return false;
            }

            requireAuthentication.CurrentUser = user;
            return true;
        }

        private void SetContext(object endpoint)
        {
            var needContext = endpoint as INeedContext;
            if (needContext != null) needContext.Context = _context;
        }

        private void RunEndpoint(AsyncEndpointRunner endpoint)
        {
            endpoint.Run().ContinueWith(RunContinuation);
        }

        private void RunContinuation(Task<Status> t)
        {
            _isCompleted = true;
            if (t.IsFaulted)
            {
                WriteError(t.Exception);
                return;
            }
            var status = t.Result;

            WriteStatusCode(status);

            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                var redirect = _runner.Endpoint as IMayRedirect;
                if (redirect != null &&
                    !string.IsNullOrWhiteSpace(redirect.Location))
                {
                    _context.Response.Headers.Set("Location", redirect.Location);
                }
            }

            var setCookies = _runner.Endpoint as ISetCookies;
            if (setCookies != null)
            {
                foreach (var cookie in setCookies.CookiesToSet)
                {
                    _context.Response.SetCookie(cookie);
                }
            }

            if (status.IsSuccess)
            {
                ResponseWriter.Write(_runner, _context);
            }

            _callback(this);
        }

        private void WriteError(Exception exception)
        {
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                _context.Response.StatusCode = httpException.ErrorCode;
                _context.Response.StatusDescription = httpException.Message;
            }
            else
            {
                Trace.TraceError(exception.Message);
                _context.Response.StatusCode = 500;
                _context.Response.StatusDescription = "Internal server error.";
            }
        }

        private void WriteStatusCode(Status status)
        {
            _context.Response.StatusCode = status.Code;
            _context.Response.StatusDescription = status.Description;
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