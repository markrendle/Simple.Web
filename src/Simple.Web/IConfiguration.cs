namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using Authentication;
    using Cors;
    using DependencyInjection;

    /// <summary>
    /// Provides configuration details for the application.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the list of public folders.
        /// </summary>
        ISet<PublicFolder> PublicFolders { get; }

        /// <summary>
        /// Gets a dictionary representing URLs which should be mapped directly to files.
        /// </summary>
        IDictionary<string, PublicFile> PublicFileMappings { get; }

        /// <summary>
        /// Gets a dictionary representing URLs which map to files but are only for authenticated users.
        /// </summary>
        IDictionary<string, PublicFile> AuthenticatedFileMappings { get; }

        /// <summary>
        /// Gets the Set of Origins which are allowed to access this application.
        /// To allow full CORS from any origin, add the wildcard &quot;*&quot; to this Set.
        /// </summary>
        ISet<IAccessControlEntry> AccessControl { get; }
            
        /// <summary>
        /// Gets or sets the IoC container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        ISimpleContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the type of the handler which provides the login page for Forms-based Authentication.
        /// </summary>
        /// <value>
        /// The login page.
        /// </value>
        Type LoginPage { get; set; }

        /// <summary>
        /// Gets or sets the authentication provider.
        /// </summary>
        /// <value>
        /// The authentication provider.
        /// </value>
        IAuthenticationProvider AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets a Handler for Exceptions to write something meaningful to the response.
        /// </summary>
        IExceptionHandler ExceptionHandler { get; set; }
    }
}