using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Web.Http;

namespace Simple.Web.Behaviors.Implementations
{
    public static class Disposable
    {
        public static void Impl(IDisposable disposable, IContext context)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
