using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    /// <summary>
    /// Represents the handler which accepts the POST operation for a login in a Forms-based authentication application.
    /// </summary>
    public interface IHandleLogin
    {
        /// <summary>
        /// Sets the URL that the application should return to once login is complete.
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        /// <remarks>
        /// This property will be set by the framework.
        /// </remarks>
        string ReturnUrl { set; }
        Guid UserToken { get; }
    }
}
