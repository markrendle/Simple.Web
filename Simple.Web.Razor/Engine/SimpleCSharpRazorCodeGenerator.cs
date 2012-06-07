namespace Simple.Web.Razor.Engine
{
    using System.CodeDom;
    using System.Web.Razor;
    using System.Web.Razor.Generator;

    public class SimpleCSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
        public SimpleCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
            var baseType = new CodeTypeReference("SimpleTemplateModelBase<dynamic>");
            Context.GeneratedClass.BaseTypes.Clear();
            Context.GeneratedClass.BaseTypes.Add(baseType);
        }
    }
}