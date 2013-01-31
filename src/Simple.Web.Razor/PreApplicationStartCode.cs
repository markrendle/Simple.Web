namespace Simple.Web.Razor
{
    using System.Web.Compilation;

    public static class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (_startWasCalled) return;
            _startWasCalled = true;
            BuildProvider.RegisterBuildProvider(".cshtml", typeof(SimpleRazorBuildProvider));
        }
    }
}
