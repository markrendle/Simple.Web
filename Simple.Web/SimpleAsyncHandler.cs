namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    internal class SimpleAsyncHandler<TEndpointType> : IHttpAsyncHandler
    {
        private readonly IContext _context;
        private readonly EndpointInfo _endpointInfo;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();
// ReSharper disable StaticFieldInGenericType
        private static readonly Lazy<RoutingTable> RoutingTable = new Lazy<RoutingTable>(() => new RoutingTableBuilder(typeof(TEndpointType)).BuildRoutingTable());
// ReSharper restore StaticFieldInGenericType

        public static IHttpHandler TryCreate(IContext context)
        {
            IDictionary<string, string> variables;
            var endpointType = RoutingTable.Value.Get(context.Request.Url.AbsolutePath, context.Request.AcceptTypes, out variables);
            if (endpointType == null) return null;
            var endpointInfo = new EndpointInfo(endpointType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                endpointInfo.Variables.Add(key, context.Request.QueryString[key]);
            }

            SimpleAsyncHandler<TEndpointType> instance;
            if (endpointInfo.RequiresAuthentication)
            {
                var authenticationProvider = SimpleWeb.Configuration.Container.Get<IAuthenticationProvider>() ??
                                             new AuthenticationProvider();
                instance = new SimpleAsyncHandler<TEndpointType>(context, endpointInfo, authenticationProvider);
            }
            else
            {
                instance = new SimpleAsyncHandler<TEndpointType>(context, endpointInfo);
            }

            return instance;
        }

        private SimpleAsyncHandler(IContext context, EndpointInfo endpointInfo) : this(context, endpointInfo, null)
        {
        }

        private SimpleAsyncHandler(IContext context, EndpointInfo endpointInfo, IAuthenticationProvider authenticationProvider)
        {
            _context = context;
            _endpointInfo = endpointInfo;
            _authenticationProvider = authenticationProvider;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var result = new SimpleAsyncHandlerResult<TEndpointType>(_context, _endpointInfo, _authenticationProvider, cb, extraData);
            result.Run();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }
    }

    class SimpleAsyncHandlerResult<TEndpointType> : IAsyncResult
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
            if (status.Code != 200)
            {
                return;
            }

            WriteResponse();

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

        private void WriteResponse()
        {
            if (_runner.HasOutput && _runner.Output is RawHtml)
            {
                _context.Response.ContentType =
                    _context.Request.AcceptTypes.FirstOrDefault(
                        at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
                _context.Response.Output.Write(_runner.Output.ToString());
            }
            else
            {
                IContentTypeHandler contentTypeHandler;
                if (!TryGetContentTypeHandler(out contentTypeHandler))
                {
                    throw new UnsupportedMediaTypeException(_context.Request.AcceptTypes);
                }
                _context.Response.ContentType = contentTypeHandler.GetContentType(_context.Request.AcceptTypes);
                contentTypeHandler.Write(new Content(_runner), _context.Response.Output);
            }
        }

        private bool TryGetContentTypeHandler(out IContentTypeHandler contentTypeHandler)
        {
            try
            {
                contentTypeHandler = _contentTypeHandlerTable.GetContentTypeHandler(_context.Request.AcceptTypes);
            }
            catch (UnsupportedMediaTypeException)
            {
                _context.Response.StatusCode = 415;
                _context.Response.Close();
                contentTypeHandler = null;
                return false;
            }
            return true;
        }
    }
}