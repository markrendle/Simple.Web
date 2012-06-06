using System.Collections.Generic;

namespace Simple.Web.Behaviors
{
    using Http;

    /// <summary>
    /// Indicates that a handler will set Cookies in a Response.
    /// </summary>
    [RequestBehavior(typeof(Implementations.SetResponseCookies))]
    public interface ISetCookies
    {
        /// <summary>
        /// Used by the Framework to set a <see cref="ICookieCollection"/> that the handler can use to modify cookies.
        /// </summary>
        /// <value>
        /// The response cookies.
        /// </value>
        IDictionary<string, ICookie> ResponseCookies { set; }
    }
}