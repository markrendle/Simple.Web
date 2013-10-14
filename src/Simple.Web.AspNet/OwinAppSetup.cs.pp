using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simple.Web;

namespace $rootnamespace$
{
	using UseAction = Action<Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>>;

    public class OwinAppSetup
    {
        public static void Setup(UseAction use)
        {
            use(Application.Run);
        }
    }
}