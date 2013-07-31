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
            var tcs = new TaskCompletionSource<Status>();
            tcs.SetException(new NotImplementedException());
            return tcs.Task;
        }

        public RawHtml Output { get; private set; }
    }
}