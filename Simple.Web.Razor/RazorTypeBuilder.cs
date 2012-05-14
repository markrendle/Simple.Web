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

        public Type CreateType(TextReader reader)
        {
            var handlerType = ExtractType(ref reader, "@handler");
            var modelType = ExtractType(ref reader, "@model");

            return CreateTypeImpl(reader, handlerType, modelType);
        }

        private static Type CreateTypeImpl(TextReader reader, Type handlerType, Type modelType)
        {
            var baseType = DecideBaseType(handlerType, modelType);

            var engine = CreateRazorTemplateEngine(baseType);

            var razorResult = engine.GenerateCode(reader);

            var compilerParameters = CreateCompilerParameters();

            var compilerResults = CompileView(razorResult, compilerParameters);

            CheckForErrors(compilerResults);

            var assembly = compilerResults.CompiledAssembly;
            return assembly.GetExportedTypes().FirstOrDefault();
        }

        private static Type DecideBaseType(Type handlerType, Type modelType)
        {
            if (handlerType == null)
            {
                if (modelType == null)
                {
                    return typeof (SimpleTemplateBase);
                }
                return typeof (SimpleTemplateModelBase<>).MakeGenericType(modelType);
            }
            if (modelType == null)
            {
                return typeof (SimpleTemplateHandlerBase<>).MakeGenericType(handlerType);
            }

            return typeof (SimpleTemplateHandlerModelBase<,>).MakeGenericType(handlerType, modelType);
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
                                     typeof (Enumerable).Assembly.GetPath(),
                                     typeof (Uri).Assembly.GetPath(),
                                     typeof (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.GetPath()
                                 };

            assemblies.AddRange(TypeResolver.KnownGoodAssemblies.Select(knownGoodAssembly => knownGoodAssembly.GetPath()));

            assemblies.AddRange(
                Assembly.GetCallingAssembly().GetReferencedAssemblies().Where(an => an.EscapedCodeBase != null).Select(
                    an => an.GetPath()));
            return assemblies.Distinct();
        }

        private static Type ExtractType(ref TextReader reader, string directive)
        {
            Type modelType = null;
            using (var writer = new StringWriter())
            {
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (line != null && line.Trim().StartsWith(directive))
                    {
                        modelType = FindTypeFromRazorLine(line, directive);
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

        private static Type FindTypeFromRazorLine(string line, string directive)
        {
            string typeName = line.Replace(directive, "").Trim();

            return TypeResolver.FindType(typeName);
        }

        internal Type CreateType(TextReader reader, Type handlerType, Type modelType)
        {
            ExtractType(ref reader, "@model");
            ExtractType(ref reader, "@handler");
            return CreateTypeImpl(reader, handlerType, modelType);
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
