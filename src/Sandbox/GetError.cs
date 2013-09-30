namespace Sandbox
{
    using System;
    using System.Threading.Tasks;

    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/error")]
    public class GetError : IGetAsync, IOutput<RawHtml>
    {
        public RawHtml Output { get; private set; }

        public Task<Status> Get()
        {
            var tcs = new TaskCompletionSource<Status>();
            tcs.SetException(new NotImplementedException());
            return tcs.Task;
        }
    }
}