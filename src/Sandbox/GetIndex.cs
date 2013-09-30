namespace Sandbox
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/")]
    public class GetIndex : IGetAsync, IOutput<RawHtml>, IDisposable
    {
        public RawHtml Output
        {
            get { return "<h2>Simple.Web is making your life easier.</h2>"; }
        }

        public void Dispose()
        {
            Trace.WriteLine("Disposing GetIndex");
        }

        public Task<Status> Get()
        {
            return DoLongRunningThing().ContinueWith(t => Status.OK);
        }

        private Task DoLongRunningThing()
        {
            return Task.Factory.StartNew(() => { });
        }
    }
}