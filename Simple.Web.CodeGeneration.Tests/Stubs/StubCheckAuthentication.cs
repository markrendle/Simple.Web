namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubCheckAuthentication
    {
        public static bool Called;
        public static bool Impl(IRequireAuthentication e, IContext c)
        {
            return Called = true;
        }
    }
}