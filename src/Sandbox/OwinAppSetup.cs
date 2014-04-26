using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sandbox
{
    using Simple.Web;
    using UseAction = Action< Func< Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task> > >;

    public class OwinAppSetup
    {
        public static void Setup(UseAction use)
        {
            use(Application.App);
        }
    }
}