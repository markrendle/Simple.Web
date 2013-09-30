namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Reflection;

    using Simple.Web.Behaviors.Implementations;

    internal class MethodLookup : IMethodLookup
    {
        public MethodInfo CheckAuthentication
        {
            get { return Get(typeof(CheckAuthentication)); }
        }

        public MethodInfo Redirect
        {
            get { return Get(typeof(Redirect)); }
        }

        public MethodInfo SetCache
        {
            get { return Get(typeof(SetCacheOptions)); }
        }

        public MethodInfo SetFiles
        {
            get { return Get(typeof(SetFiles)); }
        }

        public MethodInfo SetIfModifiedSince
        {
            get { return Get(typeof(SetIfModifiedSince)); }
        }

        public MethodInfo SetInput
        {
            get { return Get(typeof(SetInput)); }
        }

        public MethodInfo SetInputETag
        {
            get { return Get(typeof(SetInputETag)); }
        }

        public MethodInfo SetLastModified
        {
            get { return Get(typeof(SetLastModified)); }
        }

        public MethodInfo SetOutputETag
        {
            get { return Get(typeof(SetOutputETag)); }
        }

        public MethodInfo SetUserCookie
        {
            get { return Get(typeof(SetUserCookie)); }
        }

        public MethodInfo WriteOutput
        {
            get { return Get(typeof(WriteOutput)); }
        }

        public MethodInfo WriteRawHtml
        {
            get { return Get(typeof(WriteRawHtml)); }
        }

        public MethodInfo WriteStatusCode
        {
            get { return Get(typeof(WriteStatusCode)); }
        }

        public MethodInfo WriteStreamResponse
        {
            get { return Get(typeof(WriteStreamResponse)); }
        }

        public MethodInfo WriteView
        {
            get { return Get(typeof(WriteView)); }
        }

        private static MethodInfo Get(Type type)
        {
            return type.GetMethod("Impl", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }
}