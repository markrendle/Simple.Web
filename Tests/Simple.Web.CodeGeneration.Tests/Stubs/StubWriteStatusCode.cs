namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using Http;

    class StubWriteStatusCode
    {
        public static bool Called;
        public static bool Impl(Status s, IContext c)
        {
            return Called = true;
        }
    }
}