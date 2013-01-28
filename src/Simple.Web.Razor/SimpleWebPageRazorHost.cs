namespace Simple.Web.Razor
{
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;
    using Engine;

    public class SimpleWebPageRazorHost : RazorEngineHost
    {
        public SimpleWebPageRazorHost(RazorCodeLanguage language)
            : base(language)
        {
            DefaultBaseClass = "SimpleTemplateModelBase<dynamic>";
        }

        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator)
            {
                return new SimpleCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName,
                                                       incomingCodeGenerator.RootNamespaceName,
                                                       incomingCodeGenerator.SourceFileName,
                                                       incomingCodeGenerator.Host);
            }
            else
            {
                return base.DecorateCodeGenerator(incomingCodeGenerator);
            }
        }

        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser) return new SimpleCSharpCodeParser();
            return base.DecorateCodeParser(incomingCodeParser);
        }
    }
}