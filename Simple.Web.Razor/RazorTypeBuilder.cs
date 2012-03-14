namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Razor;
    using Microsoft.CSharp;

    internal class RazorTypeBuilder
    {
        internal static readonly string[] DefaultNamespaceImports = new[]
                                                                       {
                                                                           "System", "System.Text", "System.Linq",
                                                                           "System.Collections.Generic"
                                                                       };

        private static readonly TypeResolver TypeResolver = new TypeResolver();

        public Type CreateType(TextReader reader, Type modelType)
        {
            ExtractModelType(ref reader);
            return CreateTypeImpl(reader, modelType);
        }

        public Type CreateType(TextReader reader)
        {
            var modelType = ExtractModelType(ref reader);

            return CreateTypeImpl(reader, modelType);
        }

        private static Type CreateTypeImpl(TextReader reader, Type modelType)
        {
            var baseType = DecideBaseType(modelType);

            var engine = CreateRazorTemplateEngine(baseType);

            var razorResult = engine.GenerateCode(reader);

            var compilerParameters = CreateCompilerParameters();

            var compilerResults = CompileView(razorResult, compilerParameters);

            CheckForErrors(compilerResults);

            var assembly = compilerResults.CompiledAssembly;
            return assembly.GetExportedTypes().FirstOrDefault();
        }

        private static Type DecideBaseType(Type modelType)
        {
            var baseType = modelType == null
                               ? typeof (SimpleTemplateBase)
                               : typeof (SimpleTemplateBase<>).MakeGenericType(modelType);
            return baseType;
        }

        private static RazorTemplateEngine CreateRazorTemplateEngine(Type baseType)
        {
            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
                           {
                               DefaultBaseClass = baseType.FullName,
                               DefaultClassName = "SimpleTemplate",
                               DefaultNamespace = "Simple.Web.Razor",
                           };
            foreach (string nameSpace in DefaultNamespaceImports)
            {
                host.NamespaceImports.Add(nameSpace);
            }
            var engine = new RazorTemplateEngine(host);
            return engine;
        }

        private static CompilerParameters CreateCompilerParameters()
        {
            var assemblies = CreateAssembliesList();

            var compilerParameters = new CompilerParameters(assemblies.ToArray())
                                         {
                                             GenerateInMemory = true,
                                         };
            return compilerParameters;
        }

        private static CompilerResults CompileView(GeneratorResults razorResult, CompilerParameters compilerParameters)
        {
            var codeProvider = new CSharpCodeProvider();

            return codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);
        }

        private static void CheckForErrors(CompilerResults compilerResults)
        {
            var errors = compilerResults.Errors.Cast<CompilerError>().ToList();

            if (errors.Count > 0)
            {
                throw new RazorCompilerException(errors);
            }
        }

        private static IEnumerable<string> CreateAssembliesList()
        {
            var assemblies = new List<string>
                                 {
                                     Assembly.GetExecutingAssembly().GetPath(),
                                     Assembly.GetCallingAssembly().GetPath(),
                                 };

            assemblies.Add(typeof(Enumerable).Assembly.GetPath());
            assemblies.Add(typeof(Uri).Assembly.GetPath());

            assemblies.AddRange(TypeResolver.KnownGoodAssemblies.Select(knownGoodAssembly => knownGoodAssembly.GetPath()));

            assemblies.AddRange(
                Assembly.GetCallingAssembly().GetReferencedAssemblies().Where(an => an.EscapedCodeBase != null).Select(
                    an => an.GetPath()));
            return assemblies.Distinct();
        }

        private static Type ExtractModelType(ref TextReader reader)
        {
            Type modelType = null;
            using (var writer = new StringWriter())
            {
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (line != null && line.Trim().StartsWith("@model"))
                    {
                        modelType = FindModelTypeFromRazorLine(line);
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                }

                reader = new StringReader(writer.ToString());
            }
            return modelType;
        }

        private static Type FindModelTypeFromRazorLine(string line)
        {
            string typeName = line.Trim().Replace("@model ", "");

            return TypeResolver.FindType(typeName);
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

        public override string Message
        {
            get { return string.Join(Environment.NewLine, _errors); }
        }
    }
}
