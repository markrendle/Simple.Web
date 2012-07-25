using System.Collections;

namespace Simple.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstraction for an HTTP response, to be implemented by hosting.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets or sets the status code and description.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        Status Status { get; set; }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        //Stream OutputStream { get; }

        Func<Stream, CancellationToken, Task> WriteFunction { get; set; }

        /// <summary>
        /// The response headers.
        /// </summary>
        IDictionary<string, string[]> Headers { get; set; }
    }
}