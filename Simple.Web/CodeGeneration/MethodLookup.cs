namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Reflection;

    class MethodLookup : IMethodLookup
    {
        public MethodInfo CheckAuthentication { get { return Get(typeof (CheckAuthentication)); } }
        public MethodInfo SetFiles { get { return Get(typeof (SetFiles)); } }
        public MethodInfo SetRequestCookies { get { return Get(typeof (SetRequestCookies)); } }
        public MethodInfo SetResponseCookies { get { return Get(typeof (SetResponseCookies)); } }
        public MethodInfo SetInput { get { return Get(typeof (SetInput)); } }
        public MethodInfo SetInputETag { get { return Get(typeof (SetInputETag)); } } 
        public MethodInfo SetOutputETag { get { return Get(typeof (SetOutputETag)); } }
        public MethodInfo SetLastModified { get { return Get(typeof (SetLastModified)); } }
        public MethodInfo SetIfModifiedSince { get { return Get(typeof (SetIfModifiedSince)); } }
        public MethodInfo WriteStatusCode { get { return Get(typeof(WriteStatusCode)); } }
        public MethodInfo SetCache { get { return Get(typeof (SetCache)); } }
        public MethodInfo Redirect { get { return Get(typeof (Redirect)); } }
        public MethodInfo WriteStreamResponse { get { return Get(typeof (WriteStreamResponse)); } }
        public MethodInfo WriteRawHtml { get { return Get(typeof (WriteRawHtml)); } }
        public MethodInfo WriteOutput { get { return Get(typeof (WriteOutput)); } }
        public MethodInfo WriteView { get { return Get(typeof (WriteView)); } }
        public MethodInfo SetUserCookie { get { return Get(typeof (SetUserCookie)); } }

        private static MethodInfo Get(Type type)
        {
            return type.GetMethod("Impl", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }
}