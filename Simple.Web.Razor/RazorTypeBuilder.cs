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
    using System.Threading;
    using System.Web.Razor;
    using Engine;
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
            return CreateTypeImpl(reader);
        }

        private static Type CreateTypeImpl(TextReader reader)
        {
            var engine = CreateRazorTemplateEngine();

            var razorResult = engine.GenerateCode(reader);

            var compilerParameters = CreateCompilerParameters();

            var compilerResults = CompileView(razorResult, compilerParameters);

            CheckForErrors(compilerResults);

            var assembly = compilerResults.CompiledAssembly;
            return assembly.GetExportedTypes().FirstOrDefault();
        }

        private static RazorTemplateEngine CreateRazorTemplateEngine()
        {
            var language = new CSharpRazorCodeLanguage();
            var host = new SimpleRazorEngineHost(language)
                           {
                               DefaultBaseClass = "SimpleTemplateBase",
                               DefaultClassName = "SimpleView",
                               DefaultNamespace = "SimpleRazor",
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
                                     typeof(SimpleWeb).Assembly.GetPath(),
                                     Assembly.GetExecutingAssembly().GetPath(),
                                     Assembly.GetCallingAssembly().GetPath(),
                                     typeof (Enumerable).Assembly.GetPath(),
                                     typeof (Uri).Assembly.GetPath(),
                                     typeof (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.GetPath()
                                 };

            assemblies.AddRange(TypeResolver.KnownGoodAssemblies.Select(knownGoodAssembly => knownGoodAssembly.GetPath()));

            assemblies.AddRange(
                AppDomain.CurrentDomain.GetAssemblies().Where(an => (!an.GlobalAssemblyCache) && (!an.IsDynamic) && an.EscapedCodeBase != null).Select(
                    an => an.GetPath()));
            return assemblies.Distinct();
        }

        internal static Type ExtractType(ref TextReader reader, string directive)
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
            TypeHelper.ExtractType(ref reader, "@model");
            TypeHelper.ExtractType(ref reader, "@handler");
            return CreateTypeImpl(reader);
        }
    }

    static class TypeHelper
    {
        private static readonly TypeResolver TypeResolver = new TypeResolver();
        internal static Type ExtractType(ref TextReader reader, string directive)
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

                using (Interlocked.CompareExchange(ref reader, new StringReader(writer.ToString()), reader))
                {
                }
            }
            return modelType;
        }

        internal static Type DecideBaseType(Type handlerType, Type modelType)
        {
            if (handlerType == null)
            {
                if (modelType == null)
                {
                    return typeof(SimpleTemplateBase);
                }
                return typeof(SimpleTemplateModelBase<>).MakeGenericType(modelType);
            }
            if (modelType == null)
            {
                return typeof(SimpleTemplateHandlerBase<>).MakeGenericType(handlerType);
            }

            return typeof(SimpleTemplateHandlerModelBase<,>).MakeGenericType(handlerType, modelType);
        }

        private static Type FindTypeFromRazorLine(string line, string directive)
        {
            string typeName = line.Replace(directive, "").Trim();
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
