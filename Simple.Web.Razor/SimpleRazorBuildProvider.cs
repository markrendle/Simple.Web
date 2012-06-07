namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser.SyntaxTree;
    using Engine;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web | BuildProviderAppliesTo.Code)]
    public class SimpleRazorBuildProvider : BuildProvider
    {
        internal static readonly string[] DefaultNamespaceImports = new[]
                                                                       {
                                                                           "System", "System.Text", "System.Linq",
                                                                           "System.Collections.Generic", "Simple.Web", "Simple.Web.Razor"
                                                                       };
        private CodeCompileUnit _generatedCode = null;
        private RazorEngineHost _host = null;
        private IList _virtualPathDependencies;

        internal RazorEngineHost Host
        {
            get
            {
                if (_host == null)
                {
                    _host = CreateHost();
                }
                return _host;
            }
            set { _host = value; }
        }

        // Returns the base dependencies and any dependencies added via AddVirtualPathDependencies
        public override ICollection VirtualPathDependencies
        {
            get
            {
                if (_virtualPathDependencies != null)
                {
                    // Return a readonly wrapper so as to prevent users from modifying the collection directly.
                    return ArrayList.ReadOnly(_virtualPathDependencies);
                }
                else
                {
                    return base.VirtualPathDependencies;
                }
            }
        }

        public new string VirtualPath
        {
            get { return base.VirtualPath; }
        }

        public override CompilerType CodeCompilerType
        {
            get
            {
                EnsureGeneratedCode();
                CompilerType compilerType = GetDefaultCompilerTypeForLanguage(Host.CodeLanguage.LanguageName);
                return compilerType;
            }
        }

        public override Type GetGeneratedType(CompilerResults results)
        {
            return results.CompiledAssembly.GetType(String.Format(CultureInfo.CurrentCulture, "{0}.{1}", Host.DefaultNamespace, "Foot"));
        }

        internal CodeCompileUnit GeneratedCode
        {
            get
            {
                EnsureGeneratedCode();
                return _generatedCode;
            }
            set { _generatedCode = value; }
        }

        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            assemblyBuilder.AddAssemblyReference(typeof(SimpleWeb).Assembly);
            assemblyBuilder.AddAssemblyReference(typeof(SimpleTemplateBase).Assembly);
            assemblyBuilder.AddCodeCompileUnit(this, GeneratedCode);
            assemblyBuilder.GenerateTypeFactory(String.Format(CultureInfo.InvariantCulture, "{0}.{1}", Host.DefaultNamespace, "Foot"));
        }

        protected internal virtual TextReader InternalOpenReader()
        {
            return OpenReader();
        }

        private RazorEngineHost CreateHost()
        {
            var host = new SimpleRazorEngineHost(new CSharpRazorCodeLanguage())
                {
                    DefaultBaseClass = "SimpleTemplateBase",
                    DefaultClassName = "SimpleView",
                    DefaultNamespace = "SimpleRazor",
                };
            foreach (string nameSpace in DefaultNamespaceImports)
            {
                host.NamespaceImports.Add(nameSpace);
            }

            return host;
        }

        private void EnsureGeneratedCode()
        {
            if (_generatedCode == null)
            {
                var engine = new RazorTemplateEngine(Host);
                GeneratorResults results = null;
                using (TextReader reader = InternalOpenReader())
                {
                    results = engine.GenerateCode(reader);//, className: null, rootNamespace: null, sourceFileName: Host.PhysicalPath);
                }
                if (!results.Success)
                {
                    throw CreateExceptionFromParserError(results.ParserErrors.Last(), VirtualPath);
                }
                _generatedCode = results.GeneratedCode;
            }
        }

        private static HttpParseException CreateExceptionFromParserError(RazorError error, string virtualPath)
        {
            return new HttpParseException(error.Message + Environment.NewLine, null, virtualPath, null, error.Location.LineIndex + 1);
        }
    }
}