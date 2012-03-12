namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Razor;
    using Microsoft.CSharp;

    public class RazorHtmlContentTypeHandler
    {
        public Type CreateType(TextReader reader, Type modelType)
        {
            var baseType = modelType == null
                               ? typeof (SimpleTemplateBase)
                               : typeof (SimpleTemplateBase<>).MakeGenericType(modelType);

            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
                           {
                               DefaultBaseClass = baseType.FullName,
                               DefaultClassName = "SimpleTemplate",
                               DefaultNamespace = "Simple.Web.Razor"
                           };
            var engine = new RazorTemplateEngine(host);

            var razorResult = engine.GenerateCode(reader);

            var codeProvider = new CSharpCodeProvider();
            var assemblies = new List<string>
                                 {
                                     Assembly.GetExecutingAssembly().GetPath(),
                                     Assembly.GetCallingAssembly().GetPath(),
                                 };

            assemblies.AddRange(Assembly.GetCallingAssembly().GetReferencedAssemblies().Where(an => an.EscapedCodeBase != null).Select(an => an.GetPath()));

            var compilerParameters = new CompilerParameters(assemblies.Distinct().ToArray())
                                         {
                                             GenerateInMemory = true,
                                         };
            var compilerResults = codeProvider
                .CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

            var errors = compilerResults.Errors.Cast<CompilerError>().ToList();

            if (errors.Count > 0)
            {
                throw new RazorCompilerException(errors);
            }
            var assembly = compilerResults.CompiledAssembly;
            return assembly.GetExportedTypes().FirstOrDefault();
        }
    }

    public class RazorCompilerException : Exception
    {
        private readonly List<CompilerError> _errors;

        public RazorCompilerException(List<CompilerError> errors)
        {
            _errors = errors;
        }

        public ReadOnlyCollection<CompilerError> Errors
        {
            get { return _errors.AsReadOnly(); }
        }
    }
}
