namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    class StubWriteRawHtml
    {
        public static bool Called;
        public static bool Impl(IOutput<RawHtml> e, IContext c)
        {
            return Called = true;
        }
    }
}