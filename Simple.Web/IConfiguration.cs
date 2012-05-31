namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using DependencyInjection;

    /// <summary>
    /// Provides configuration details for the application.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the list of public folders.
        /// </summary>
        ISet<string> PublicFolders { get; }

        /// <summary>
        /// Gets a dictionary representing URLs which should be mapped directly to files.
        /// </summary>
        IDictionary<string, string> PublicFileMappings { get; }

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
    }
}