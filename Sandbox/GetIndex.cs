using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox
{
    using System.Threading.Tasks;
    using Simple.Web;

    [UriTemplate("/")]
    public class GetIndex : IGetAsync
    {
        public Task<Status> Get()
        {
            return Task.Factory.StartNew(() => Status.OK);
        }

        public object Output
        {
            get { return Raw.Html("Simple.Web has entered the building."); }
        }
    }
}