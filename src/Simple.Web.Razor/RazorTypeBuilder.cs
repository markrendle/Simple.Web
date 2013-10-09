namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Razor;

    using Microsoft.CSharp;

    using Simple.Web.Razor.Engine;

    internal class RazorTypeBuilder
    {
        private static readonly IDictionary<String, String> CompilerProperties =
            new Dictionary<String, String> { { "CompilerVersion", "v4.0" } };

        public Type CreateType(StreamReader reader)
        {
            return CreateTypeImpl(reader);
        }

        private static Type CreateTypeImpl(StreamReader reader)
        {
            var context = new RazorTypeBuilderContext();

            var engine = CreateRazorTemplateEngine();
            var razorResult = engine.GenerateCode(reader, context.ClassName, engine.Host.DefaultNamespace, null);
            var viewType = CompileView(razorResult, context.GetCompilerParameters());

            return viewType;
        }

        private static RazorTemplateEngine CreateRazorTemplateEngine()
        {
            var language = new CSharpRazorCodeLanguage();
            var host = new SimpleRazorEngineHost(language);
            var engine = new RazorTemplateEngine(host);

            return engine;
        }

        private static Type CompileView(GeneratorResults razorResult, CompilerParameters compilerParameters)
        {
            var codeProvider = new CSharpCodeProvider(CompilerProperties);
            var result = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

            if (result.Errors != null && result.Errors.HasErrors)
            {
                throw new RazorCompilerException(result.Errors.OfType<CompilerError>().Where(x => !x.IsWarning));
            }

            var assembly = Assembly.LoadFrom(compilerParameters.OutputAssembly);

            if (assembly == null)
            {
                throw new RazorCompilerException("Unable to load view assembly.");
            }

            var type = assembly.GetType(SimpleRazorConfiguration.Namespace + "." + compilerParameters.MainClass);

            if (type == null)
            {
                throw new RazorCompilerException("Unable to load view assembly.");
            }

            return type;
        }
    }
}
