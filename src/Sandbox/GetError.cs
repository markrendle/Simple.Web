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
    public class GetError : IGetAsync, IOutput<RawHtml>
    {
        public Task<Status> Get()
        {
            return Task.Factory.StartNew<Status>(() =>
                {
                    throw new NotImplementedException();
                });
        }

        public RawHtml Output { get; private set; }
    }
}