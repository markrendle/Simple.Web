using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    public interface IEndpoint
    {
        string UriTemplate { get; }
        object Run();
    }
}
