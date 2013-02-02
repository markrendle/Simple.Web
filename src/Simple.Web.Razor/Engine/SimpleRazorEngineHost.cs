namespace Simple.Web.Razor.Engine
{
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    internal class SimpleRazorEngineHost : RazorEngineHost
    {
        public SimpleRazorEngineHost(RazorCodeLanguage codeLanguage) : base(codeLanguage)
        {
            this.DefaultBaseClass = SimpleRazorConfiguration.BaseClass;
            this.DefaultClassName = SimpleRazorConfiguration.ClassPrefix;
            this.DefaultNamespace = SimpleRazorConfiguration.Namespace;

            base.GeneratedClassContext = new GeneratedClassContext(
                executeMethodName: "Execute", 
                writeMethodName: "Write", 
                writeLiteralMethodName: "WriteLiteral", 
                writeToMethodName: "WriteTo", 
                writeLiteralToMethodName: "WriteLiteralTo",
                templateTypeName: null,
                defineSectionMethodName: "DefineSection");

            foreach (var namespaceAssembly in SimpleRazorConfiguration.NamespaceImports)
            {
                this.NamespaceImports.Add(namespaceAssembly.Key);
            }
        }

        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser)
            {
                return new SimpleCSharpCodeParser();
            }
            return base.DecorateCodeParser(incomingCodeParser);
        }

        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator)
            {
                return new SimpleCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, this);
            }
            return base.DecorateCodeGenerator(incomingCodeGenerator);
        }
    }
}
