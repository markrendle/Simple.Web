namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Reflection;

    class MethodLookup : IMethodLookup
    {
        public MethodInfo CheckAuthentication { get { return Get(typeof (CheckAuthentication)); } }
        public MethodInfo SetFiles { get { return Get(typeof (SetFiles)); } }
        public MethodInfo SetInput { get { return Get(typeof (SetInput)); } }

        public MethodInfo WriteStatusCode
        {
            get { return Get(typeof(WriteStatusCode)); }
        }

        public MethodInfo SetCookies { get { return Get(typeof (SetCookies)); } }
        public MethodInfo DisableCache { get { return Get(typeof (DisableCache)); } }
        public MethodInfo Redirect { get { return Get(typeof (Redirect)); } }
        public MethodInfo WriteStreamResponse { get { return Get(typeof (WriteStreamResponse)); } }
        public MethodInfo WriteRawHtml { get { return Get(typeof (WriteRawHtml)); } }
        public MethodInfo WriteOutput { get { return Get(typeof (WriteOutput)); } }
        public MethodInfo WriteView { get { return Get(typeof (WriteView)); } }

        private static MethodInfo Get(Type type)
        {
            return type.GetMethod("Impl", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }
}