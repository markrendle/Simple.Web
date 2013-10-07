namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using Authentication;
    using Cors;
    using DependencyInjection;
    using MediaTypeHandling;

    /// <summary>
    ///     Default implementation of <see cref="IConfiguration" />.
    /// </summary>
    public sealed class Configuration : IConfiguration
    {
        private readonly IDictionary<string, PublicFile> _authenticatedFileMappings = new FileMappingDictionary();

        private readonly DefaultAuthenticationProvider _defaultAuthenticationProvider =
            new DefaultAuthenticationProvider();

        private readonly IDictionary<string, PublicFile> _publicFileMappings = new FileMappingDictionary();

        private readonly HashSet<PublicFolder> _publicFolders = new HashSet<PublicFolder>();
        private ISimpleContainer _container = new DefaultSimpleContainer();
        private IAuthenticationProvider _authenticationProvider;
        private IExceptionHandler _exceptionHandler;

        /// <summary>
        ///     Gets a dictionary representing URLs which should be mapped directly to files.
        /// </summary>
        public IDictionary<string, PublicFile> PublicFileMappings
        {
            get { return _publicFileMappings; }
        }

        /// <summary>
        ///     Gets the list of public folders.
        /// </summary>
        public ISet<PublicFolder> PublicFolders
        {
            get { return _publicFolders; }
        }
        private readonly ISet<IAccessControlEntry> _accessControl = new HashSet<IAccessControlEntry>(AccessControlEntry.OriginComparer);

        /// <summary>
        ///     Gets a dictionary representing URLs which map to files but are only for authenticated users.
        /// </summary>
        public IDictionary<string, PublicFile> AuthenticatedFileMappings
        {
            get { return _authenticatedFileMappings; }
        }

        /// <summary>
        ///     Gets or sets the IoC container.
        /// </summary>
        /// <value>
        ///     The container.
        /// </value>
        public ISimpleContainer Container
        {
            get { return _container; }
            set { _container = value ?? new DefaultSimpleContainer(); }
        }

        public ISet<IAccessControlEntry> AccessControl { get { return _accessControl; } }

        /// <summary>
        ///     Gets or sets the type of the handler which provides the login page for Forms-based Authentication.
        /// </summary>
        /// <value>
        ///     The login page.
        /// </value>
        public Type LoginPage { get; set; }

        /// <summary>
        ///     Gets or sets the authentication provider.
        /// </summary>
        /// <value>
        ///     The authentication provider.
        /// </value>
        public IAuthenticationProvider AuthenticationProvider
        {
            get { return _authenticationProvider ?? CreateAuthenticationProvider() ?? _defaultAuthenticationProvider; }
            set { _authenticationProvider = value; }
        }

        private IAuthenticationProvider CreateAuthenticationProvider()
        {
            try
            {
                return _authenticationProvider = Container.BeginScope().Get<IAuthenticationProvider>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IMediaTypeHandler DefaultMediaTypeHandler { get; set; }

        public IExceptionHandler ExceptionHandler
        {
            get { return _exceptionHandler ?? CreateExceptionHandler(); }
            set { _exceptionHandler = value; }
        }

        private IExceptionHandler CreateExceptionHandler()
        {
            try
            {
                return _exceptionHandler = Container.BeginScope().Get<IExceptionHandler>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}