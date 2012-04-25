namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubSetCookies
    {
        public static bool Called;
        public static bool Impl(ISetCookies e, IContext c)
        {
            return Called = true;
        }
    }
}