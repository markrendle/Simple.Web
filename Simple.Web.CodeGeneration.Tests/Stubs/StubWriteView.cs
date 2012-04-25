namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubWriteView
    {
        public static bool Called;
        public static bool Impl(ISpecifyView e, IContext c)
        {
            return Called = true;
        }
    }
}