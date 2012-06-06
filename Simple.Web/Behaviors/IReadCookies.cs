using System.Collections.Generic;

namespace Simple.Web.Behaviors
{
    using Http;

    /// <summary>
    /// Indicates that a handler requires access to the Cookies from a Request.
    /// </summary>
    [RequestBehavior(typeof(Implementations.SetRequestCookies))]
    public interface IReadCookies
    {
        /// <summary>
        /// Used by the framework to set the request cookies.
        /// </summary>
        /// <value>
        /// The request cookies.
        /// </value>
        IDictionary<string,ICookie> RequestCookies { set; }
    }
}