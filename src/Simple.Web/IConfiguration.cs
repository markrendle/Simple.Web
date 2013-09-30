namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    using Simple.Web.Authentication;
    using Simple.Web.Cors;
    using Simple.Web.DependencyInjection;
    using Simple.Web.MediaTypeHandling;

    /// <summary>
    /// Provides configuration details for the application.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the Set of Origins which are allowed to access this application.
        /// To allow full CORS from any origin, add the wildcard &quot;*&quot; to this Set.
        /// </summary>
        ISet<IAccessControlEntry> AccessControl { get; }

        /// <summary>
        /// Gets a dictionary representing URLs which map to files but are only for authenticated users.
        /// </summary>
        IDictionary<string, PublicFile> AuthenticatedFileMappings { get; }

        /// <summary>
        /// Gets or sets the authentication provider.
        /// </summary>
        /// <value>
        /// The authentication provider.
        /// </value>
        IAuthenticationProvider AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets the IoC container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        ISimpleContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the MediaTypeHandler to use when Accept is */*
        /// </summary>
        /// <value>
        /// An <see cref="IMediaTypeHandler"/> instance
        /// </value>
        IMediaTypeHandler DefaultMediaTypeHandler { get; set; }

        /// <summary>
        /// Gets or sets a Handler for Exceptions to write something meaningful to the response.
        /// </summary>
        IExceptionHandler ExceptionHandler { get; set; }

        /// <summary>
        /// Gets or sets the type of the handler which provides the login page for Forms-based Authentication.
        /// </summary>
        /// <value>
        /// The login page.
        /// </value>
        Type LoginPage { get; set; }

        /// <summary>
        /// Gets a dictionary representing URLs which should be mapped directly to files.
        /// </summary>
        IDictionary<string, PublicFile> PublicFileMappings { get; }

        /// <summary>
        /// Gets the list of public folders.
        /// </summary>
        ISet<PublicFolder> PublicFolders { get; }
    }
}