namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubDisableCache
    {
        public static bool Called;
        public static bool Impl(IContext c)
        {
            return Called = true;
        }
    }
}