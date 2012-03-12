namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;

    internal interface IRequest
    {
        Uri Url { get; }
        IList<string> AcceptTypes { get; }
        NameValueCollection QueryString { get; }
        Stream InputStream { get; }
        string ContentType { get; }
        string HttpMethod { get; }
    }
}