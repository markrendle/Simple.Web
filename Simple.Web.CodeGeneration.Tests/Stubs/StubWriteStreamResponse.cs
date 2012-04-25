namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubWriteStreamResponse
    {
        public static bool Called;
        public static bool Impl(IOutputStream e, IContext c)
        {
            return Called = true;
        }
    }
}