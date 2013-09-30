namespace Simple.Web.OwinSupport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Simple.Web.Http;

    internal class OwinResponse : IResponse
    {
        public IDictionary<string, string[]> Headers { get; set; }

        public Status Status { get; set; }

        public Func<Stream, Task> WriteFunction { get; set; }
    }
}