namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubSetResponseCookies
    {
        public static bool Called;
        public static bool Impl(ISetCookies e, IContext c)
        {
            return Called = true;
        }
    }
    class StubSetRequestCookies
    {
        public static bool Called;
        public static bool Impl(IReadCookies e, IContext c)
        {
            return Called = true;
        }
    }
}