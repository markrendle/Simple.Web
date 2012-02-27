using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    public abstract class GetEndpoint
    {
        protected internal abstract string UriTemplate { get; }
        protected internal abstract object Run();
    }
}
