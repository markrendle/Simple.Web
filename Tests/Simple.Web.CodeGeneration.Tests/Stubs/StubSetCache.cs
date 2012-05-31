namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using Http;

    class StubSetCache
    {
        public static bool Called;
        public static bool Impl(IContext c)
        {
            return Called = true;
        }
    }
}