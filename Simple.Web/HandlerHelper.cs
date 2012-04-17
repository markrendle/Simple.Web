namespace Simple.Web
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;

    internal class HandlerHelper
    {
        private readonly EndpointInfo _endpointInfo;
        private readonly IContext _context;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ContentTypeHandlerTable _contentTypeHandlerTable = new ContentTypeHandlerTable();

        public HandlerHelper(EndpointInfo endpointInfo, IContext context, IAuthenticationProvider authenticationProvider)
        {
            _endpointInfo = endpointInfo;
            _context = context;
            _authenticationProvider = authenticationProvider;
        }

        internal bool CheckAuthentication(object endpoint)
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

        internal void SetContext(object endpoint)
        {
            var needContext = endpoint as INeedContext;
            if (needContext != null) needContext.Context = _context;
        }

        internal void SetFiles(object endpoint)
        {
            if (_context.Request.Files.Any())
            {
                var uploadFiles = endpoint as IUploadFiles;
                if (uploadFiles != null)
                {
                    uploadFiles.Files = _context.Request.Files;
                }
            }
        }

        internal void WriteResponse(IEndpointRunner runner, Status status)
        {
            WriteStatusCode(status);

            SetCookies(runner.Endpoint as ISetCookies);

            if ((status.Code >= 301 && status.Code <= 303) || status.Code == 307)
            {
                Redirect(runner.Endpoint as IMayRedirect);
            }
            else if (status.IsSuccess)
            {
                ResponseWriter.Write(runner, _context);
            }
        }

        private void Redirect(IMayRedirect redirect)
        {
            if (redirect != null &&
                !string.IsNullOrWhiteSpace(redirect.Location))
            {
                _context.Response.Headers.Set("Location", redirect.Location);
            }
            else
            {
                throw new InvalidOperationException("Redirect status returned with no Location.");
            }
        }

        private void SetCookies(ISetCookies setCookies)
        {
            if (setCookies != null)
            {
                foreach (var cookie in setCookies.CookiesToSet)
                {
                    _context.Response.SetCookie(cookie);
                }
            }
        }

        internal void WriteError(Exception exception)
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
    }
}