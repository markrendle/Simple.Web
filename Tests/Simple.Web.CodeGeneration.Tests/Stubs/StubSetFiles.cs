namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using Behaviors;
    using Http;

    class StubSetFiles
    {
        public static bool Called;
        public static bool Impl(IUploadFiles e, IContext c)
        {
            return Called = true;
        }
    }
}