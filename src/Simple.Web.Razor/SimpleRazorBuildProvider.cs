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
    using Engine;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web | BuildProviderAppliesTo.Code)]
    public class SimpleRazorBuildProvider : BuildProvider
    {
        private readonly RazorCodeLanguage _codeLanguage;
        private readonly CompilerType _compilerType;
        private readonly RazorEngineHost _host;
        private readonly IList _virtualPathDependencies;
        private readonly string _typeName;

        private CodeCompileUnit _generatedCode;

        public SimpleRazorBuildProvider()
        {
            this._codeLanguage = new CSharpRazorCodeLanguage();
            this._compilerType = GetDefaultCompilerTypeForLanguage(this._codeLanguage.LanguageName);
            this._host = new SimpleRazorEngineHost(this._codeLanguage);
            this._virtualPathDependencies = null;
            this._typeName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this._host.DefaultNamespace, "Foot");
        }

        public override ICollection VirtualPathDependencies
        {
            get
            {
                return _virtualPathDependencies != null
                    ? ArrayList.ReadOnly(_virtualPathDependencies)
                    : base.VirtualPathDependencies;
            }
        }

        public override CompilerType CodeCompilerType
        {
            get
            {
                return _compilerType;
            }
        }

        public override Type GetGeneratedType(CompilerResults results)
        {
            return results.CompiledAssembly.GetType(this._typeName);
        }

        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            if (this._generatedCode == null)
            {
                this._generatedCode = GenerateCode();
            }

            assemblyBuilder.AddAssemblyReference(typeof(SimpleWeb).Assembly);
            assemblyBuilder.AddAssemblyReference(typeof(SimpleTemplateBase).Assembly);
            assemblyBuilder.AddCodeCompileUnit(this, this._generatedCode);
            assemblyBuilder.GenerateTypeFactory(this._typeName);
        }

        private CodeCompileUnit GenerateCode()
        {
            var engine = new RazorTemplateEngine(this._host);
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