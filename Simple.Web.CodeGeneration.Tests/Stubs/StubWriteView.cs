namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubWriteView
    {
        public static bool Called;
        public static bool Impl(object e, IContext c)
        {
            return Called = true;
        }
    }
}