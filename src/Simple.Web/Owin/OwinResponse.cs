namespace Simple.Web.Owin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Http;

    internal class OwinResponse : IResponse
    {
        public Status Status { get; set; }
        public Func<Stream, Task> WriteFunction { get; set; }
        public IDictionary<string, string[]> Headers { get; set; }
    }
}