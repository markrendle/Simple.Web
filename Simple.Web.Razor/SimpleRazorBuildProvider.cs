namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Web.Compilation;
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web)]
    public class SimpleRazorBuildProvider : BuildProvider
    {
        private CompilerType _compilerType;

        public SimpleRazorBuildProvider()
        {
            using (var writer = new StreamWriter(@"C:\Temp\BPlog.txt", false))
            {
                writer.WriteLine("Bastard");
            }
        }

        public override CompilerType CodeCompilerType
        {
            get
            {
                Log("Get CodeCompilerType");
                var type = _compilerType ?? (_compilerType = GetDefaultCompilerTypeForLanguage("C#"));
                Log("Got CodeCompilerType");
                return type;
            }
        }

        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            Log("GenerateCode");
            var reader = this.OpenReader();
            var handlerType = TypeHelper.ExtractType(ref reader, "@handler");
            var modelType = TypeHelper.ExtractType(ref reader, "@model");
            var baseType = TypeHelper.DecideBaseType(handlerType, modelType);

            using (var writer = new StreamWriter(@"C:\Temp\BP.txt", true))
            {
                writer.WriteLine(handlerType);
                writer.WriteLine(modelType);
            }

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
                {
                    DefaultBaseClass = baseType.FullName,
                    DefaultNamespace = "SimpleRazor",
                    DefaultClassName = "SimpleRazorView"
                };

            var engine = new RazorTemplateEngine(host);
            var code = engine.GenerateCode(reader);

            reader.Dispose();

            if (!code.Success)
            {
                throw new RazorParserException(code.ParserErrors);
            }

            assemblyBuilder.AddCodeCompileUnit(this, code.GeneratedCode);
            assemblyBuilder.GenerateTypeFactory("SimpleRazor.SimpleRazorView");
        }

        public override Type GetGeneratedType(System.CodeDom.Compiler.CompilerResults results)
        {
            Log("GetGeneratedType");
            var generatedType = results.CompiledAssembly.GetType("SimpleRazor.SimpleRazorView");
            if (generatedType == null)
            {
                Log("No worky");
            }
            else
            {
                Log(generatedType.Name);
            }
            return generatedType;
        }

        public override void ProcessCompileErrors(System.CodeDom.Compiler.CompilerResults results)
        {
            Log("ProcessCompileErrors");
            base.ProcessCompileErrors(results);
        }

        protected override System.CodeDom.CodeCompileUnit GetCodeCompileUnit(out System.Collections.IDictionary linePragmasTable)
        {
            Log("GetCodeCompileUnit");
            var codeCompileUnit = base.GetCodeCompileUnit(out linePragmasTable);
            Log("GotCodeCompileUnit");
            return codeCompileUnit;
        }

        public override string GetCustomString(System.CodeDom.Compiler.CompilerResults results)
        {
            Log("GetCustommString");
            return base.GetCustomString(results);
        }

        public override BuildProviderResultFlags GetResultFlags(System.CodeDom.Compiler.CompilerResults results)
        {
            Log("GetResultFlags");
            return base.GetResultFlags(results);
        }

        private static void Log(string text)
        {
            using (var writer = new StreamWriter(@"C:\Temp\BPlog.txt", true))
            {
                writer.WriteLine(text);
            }
        }
    }

    public class RazorParserException : Exception
    {
        private readonly IList<RazorError> _errors;

        public RazorParserException(IList<RazorError> errors)
        {
            _errors = errors;
        }

        public IList<RazorError> Errors
        {
            get { return _errors; }
        }
    }
}