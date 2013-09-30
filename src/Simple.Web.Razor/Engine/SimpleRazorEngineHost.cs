namespace Simple.Web.Razor.Engine
{
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    internal class SimpleRazorEngineHost : RazorEngineHost
    {
        public SimpleRazorEngineHost(RazorCodeLanguage codeLanguage)
            : base(codeLanguage)
        {
            DefaultBaseClass = SimpleRazorConfiguration.BaseClass;
            DefaultClassName = SimpleRazorConfiguration.ClassPrefix;
            DefaultNamespace = SimpleRazorConfiguration.Namespace;

            base.GeneratedClassContext = new GeneratedClassContext("Execute",
                                                                   "Write",
                                                                   "WriteLiteral",
                                                                   "WriteTo",
                                                                   "WriteLiteralTo",
                                                                   null,
                                                                   "DefineSection");

            foreach (var namespaceAssembly in SimpleRazorConfiguration.NamespaceImports)
            {
                NamespaceImports.Add(namespaceAssembly.Key);
            }
        }

        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator)
            {
                return new SimpleCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName,
                                                          incomingCodeGenerator.RootNamespaceName,
                                                          incomingCodeGenerator.SourceFileName,
                                                          this);
            }
            return base.DecorateCodeGenerator(incomingCodeGenerator);
        }

        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser)
            {
                return new SimpleCSharpCodeParser();
            }
            return base.DecorateCodeParser(incomingCodeParser);
        }
    }
}