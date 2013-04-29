using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using System.Threading.Tasks;
    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/error")]
    public class GetError : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            throw new NotImplementedException();
        }

        public RawHtml Output { get; private set; }
    }
}