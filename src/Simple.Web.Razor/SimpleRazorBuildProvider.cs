namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;

    using Simple.Web.Razor.Engine;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web | BuildProviderAppliesTo.Code)]
    public class SimpleRazorBuildProvider : BuildProvider
    {
        private readonly RazorCodeLanguage _codeLanguage;
        private readonly CompilerType _compilerType;
        private readonly RazorEngineHost _host;
        private readonly string _typeName;
        private readonly IList _virtualPathDependencies;

        private CodeCompileUnit _generatedCode;

        public SimpleRazorBuildProvider()
        {
            _codeLanguage = new CSharpRazorCodeLanguage();
            _compilerType = GetDefaultCompilerTypeForLanguage(_codeLanguage.LanguageName);
            _host = new SimpleRazorEngineHost(_codeLanguage);
            _virtualPathDependencies = null;
            _typeName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _host.DefaultNamespace, "Foot");
        }

        public override CompilerType CodeCompilerType
        {
            get { return _compilerType; }
        }

        public override ICollection VirtualPathDependencies
        {
            get { return _virtualPathDependencies != null ? ArrayList.ReadOnly(_virtualPathDependencies) : base.VirtualPathDependencies; }
        }

        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            if (_generatedCode == null)
            {
                _generatedCode = GenerateCode();
            }

            assemblyBuilder.AddAssemblyReference(typeof(SimpleWeb).Assembly);
            assemblyBuilder.AddAssemblyReference(typeof(SimpleTemplateBase).Assembly);
            assemblyBuilder.AddCodeCompileUnit(this, _generatedCode);
            assemblyBuilder.GenerateTypeFactory(_typeName);
        }

        public override Type GetGeneratedType(CompilerResults results)
        {
            return results.CompiledAssembly.GetType(_typeName);
        }

        private CodeCompileUnit GenerateCode()
        {
            var engine = new RazorTemplateEngine(_host);
            GeneratorResults results;
            using (TextReader reader = OpenReader())
            {
                results = engine.GenerateCode(reader); //, className: null, rootNamespace: null, sourceFileName: Host.PhysicalPath);
            }

            if (!results.Success)
            {
                throw CreateExceptionFromParserError(results.ParserErrors.Last(), VirtualPath);
            }

            return results.GeneratedCode;
        }

        private static HttpParseException CreateExceptionFromParserError(RazorError error, string virtualPath)
        {
            return new HttpParseException(error.Message + Environment.NewLine, null, virtualPath, null, error.Location.LineIndex + 1);
        }
    }
}