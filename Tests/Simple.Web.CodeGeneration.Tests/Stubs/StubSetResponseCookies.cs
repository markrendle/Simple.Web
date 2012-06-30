namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using Behaviors;
    using Http;

    class StubSetUserCookie
    {
        public static bool Called;
        public static bool Impl(ILogin e, IContext c)
        {
            return Called = true;
        }
    }
}