using System.Reflection;

namespace Simple.Web.Razor
{
    using System.Collections.Generic;

    internal static class SimpleRazorConfiguration
    {
        internal const string ClassPrefix = "SimpleView";

        internal const string Namespace = "SimpleRazor";

        internal static readonly string BaseClass = typeof(SimpleTemplateBase).FullName;

        internal static readonly IDictionary<string, Assembly> NamespaceImports = new Dictionary<string, Assembly>()
        {
            { "System", null },
            { "System.Text", typeof(System.Text.StringBuilder).Assembly },
            { "System.Linq", typeof(System.Linq.Enumerable).Assembly },
            { "System.Collections.Generic", typeof(System.Collections.Generic.IDictionary<,>).Assembly },
            { "Simple.Web", typeof(Simple.Web.SimpleWeb).Assembly },
            { "Simple.Web.Razor", typeof(Simple.Web.Razor.SimpleTemplateBase).Assembly },
            { "Microsoft.CSharp.RuntimeBinder", typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly },
        };
    }
}