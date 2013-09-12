using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using System.Threading.Tasks;
    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/")]
    public class GetIndex : IGetAsync, IOutput<RawHtml>, IDisposable
    {
        public Task<Status> Get()
        {
            return DoLongRunningThing()
                .ContinueWith(t => Status.OK);
        }

        public RawHtml Output
        {
            get { return "<h2>Simple.Web is making your life easier.</h2>"; }
        }

        private Task DoLongRunningThing()
        {
            return Task.Factory.StartNew(() => { });
        }

        public void Dispose()
        {
            Trace.WriteLine("Disposing GetIndex");
        }
    }
}