namespace Simple.Web.Razor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

    internal static class SimpleRazorConfiguration
    {
        internal const string ClassPrefix = "SimpleView";

        internal const string Namespace = "SimpleRazor";

        internal static readonly string BaseClass = typeof(SimpleTemplateBase).FullName;

        internal static readonly IDictionary<string, Assembly> NamespaceImports = new Dictionary<string, Assembly>
            {
                { "System", null },
                { "System.Text", typeof(StringBuilder).Assembly },
                { "System.Linq", typeof(Enumerable).Assembly },
                { "System.Collections.Generic", typeof(IDictionary<,>).Assembly },
                { "Simple.Web", typeof(SimpleWeb).Assembly },
                { "Simple.Web.Razor", typeof(SimpleTemplateBase).Assembly },
                { "Microsoft.CSharp.RuntimeBinder", typeof(Binder).Assembly },
            };
    }
}