namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using Behaviors;
    using Http;

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
    class StubSetUserCookie
    {
        public static bool Called;
        public static bool Impl(ILogin e, IContext c)
        {
            return Called = true;
        }
    }
}