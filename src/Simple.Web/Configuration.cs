namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    using Simple.Web.Authentication;
    using Simple.Web.Cors;
    using Simple.Web.DependencyInjection;
    using Simple.Web.MediaTypeHandling;

    /// <summary>
    ///     Default implementation of <see cref="IConfiguration" />.
    /// </summary>
    public sealed class Configuration : IConfiguration
    {
        private readonly ISet<IAccessControlEntry> _accessControl = new HashSet<IAccessControlEntry>(AccessControlEntry.OriginComparer);
        private readonly IDictionary<string, PublicFile> _authenticatedFileMappings = new FileMappingDictionary();

        private readonly DefaultAuthenticationProvider _defaultAuthenticationProvider = new DefaultAuthenticationProvider();

        private readonly IDictionary<string, PublicFile> _publicFileMappings = new FileMappingDictionary();

        private readonly HashSet<PublicFolder> _publicFolders = new HashSet<PublicFolder>();
        private IAuthenticationProvider _authenticationProvider;
        private ISimpleContainer _container = new DefaultSimpleContainer();

        public ISet<IAccessControlEntry> AccessControl
        {
            get { return _accessControl; }
        }

        /// <summary>
        ///     Gets a dictionary representing URLs which map to files but are only for authenticated users.
        /// </summary>
        public IDictionary<string, PublicFile> AuthenticatedFileMappings
        {
            get { return _authenticatedFileMappings; }
        }

        /// <summary>
        ///     Gets or sets the authentication provider.
        /// </summary>
        /// <value>
        ///     The authentication provider.
        /// </value>
        public IAuthenticationProvider AuthenticationProvider
        {
            get { return _authenticationProvider ?? _defaultAuthenticationProvider; }
            set { _authenticationProvider = value; }
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

        public IMediaTypeHandler DefaultMediaTypeHandler { get; set; }

        public IExceptionHandler ExceptionHandler { get; set; }

        /// <summary>
        ///     Gets or sets the type of the handler which provides the login page for Forms-based Authentication.
        /// </summary>
        /// <value>
        ///     The login page.
        /// </value>
        public Type LoginPage { get; set; }

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
    }
}