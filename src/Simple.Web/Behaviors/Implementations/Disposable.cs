namespace Simple.Web.Behaviors.Implementations
{
    using System;

    using Simple.Web.Http;

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