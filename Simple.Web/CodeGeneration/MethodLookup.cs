namespace Simple.Web.CodeGeneration
{
    using System.Reflection;

    class MethodLookup : IMethodLookup
    {
        public MethodInfo CheckAuthentication { get { return typeof (CheckAuthentication).GetMethod("Impl"); } }
        public MethodInfo SetFiles { get { return typeof (SetFiles).GetMethod("Impl"); } }
        public MethodInfo SetInput { get { return typeof (SetInput).GetMethod("Impl"); } }
        public MethodInfo WriteStatusCode { get { return typeof (WriteStatusCode).GetMethod("Impl"); } }
        public MethodInfo SetCookies { get { return typeof (SetCookies).GetMethod("Impl"); } }
        public MethodInfo DisableCache { get { return typeof (DisableCache).GetMethod("Impl"); } }
        public MethodInfo Redirect { get { return typeof (Redirect).GetMethod("Impl"); } }
        public MethodInfo WriteStreamResponse { get { return typeof (WriteStreamResponse).GetMethod("Impl"); } }
        public MethodInfo WriteRawHtml { get { return typeof (WriteRawHtml).GetMethod("Impl"); } }
        public MethodInfo WriteOutput { get { return typeof (WriteOutput).GetMethod("Impl"); } }
        public MethodInfo WriteView { get { return typeof (WriteView).GetMethod("Impl"); } }
    }
}