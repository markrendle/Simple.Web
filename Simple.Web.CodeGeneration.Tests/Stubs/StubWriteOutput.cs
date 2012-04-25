namespace Simple.Web.CodeGeneration.Tests.Stubs
{
    using System;

    class StubWriteOutput
    {
        public static bool Called;
        public static Type WithType;
        public static bool Impl<T>(IOutput<T> e, IContext c)
        {
            WithType = typeof(T);
            return Called = true;
        }
    }
}