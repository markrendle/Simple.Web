namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubSetOutputETag
    {
        public static bool Called;
        public static bool Impl(IETag e, IContext c)
        {
            return Called = true;
        }
    }
}