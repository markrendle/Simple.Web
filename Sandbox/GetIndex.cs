using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using System.Threading.Tasks;
    using Simple.Web;

    [UriTemplate("/")]
    public class GetIndex : IGetAsync, IOutput<RawHtml>
    {
        //public Status Get()
        //{
        //    return Status.OK;
        //}

        public Task<Status> Get()
        {
            return Task.Factory.StartNew(() => Status.OK);
        }

        public RawHtml Output
        {
            get { return Raw.Html("Simple.Web has entered the building."); }
        }
    }
}