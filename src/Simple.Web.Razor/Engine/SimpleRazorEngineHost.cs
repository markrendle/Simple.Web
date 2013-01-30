using System.Collections.Generic;

namespace Simple.Web.Razor.Engine
{
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    internal class SimpleRazorEngineHost : RazorEngineHost
    {
        public SimpleRazorEngineHost(RazorCodeLanguage codeLanguage) : base(codeLanguage)
        {
            base.DefaultBaseClass = SimpleRazorConfiguration.BaseClass;
            base.DefaultClassName = SimpleRazorConfiguration.ClassName;
            base.DefaultNamespace = SimpleRazorConfiguration.Namespace;

            foreach (var namespaceAssembly in SimpleRazorConfiguration.NamespaceImports)
            {
                base.NamespaceImports.Add(namespaceAssembly.Key);
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
