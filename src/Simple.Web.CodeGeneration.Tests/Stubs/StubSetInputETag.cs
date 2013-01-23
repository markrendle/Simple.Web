namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using Behaviors;
    using Http;

    class StubSetInputETag
    {
        public static bool Called;
        public static bool Impl(IETag e, IContext c)
        {
            return Called = true;
        }
    }
}