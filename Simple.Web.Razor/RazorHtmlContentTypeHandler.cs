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
            var baseType = modelType == null
                               ? typeof (SimpleTemplateBase)
                               : typeof (SimpleTemplateBase<>).MakeGenericType(modelType);

            var engine = CreateRazorTemplateEngine(baseType);

            var razorResult = engine.GenerateCode(reader);

            var assemblies = CreateAssembliesList(modelType);

            var compilerParameters = new CompilerParameters(assemblies.Distinct().ToArray())
                                         {
                                             GenerateInMemory = true,
                                         };

            var codeProvider = new CSharpCodeProvider();

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

        private static List<string> CreateAssembliesList(Type modelType)
        {
            var assemblies = new List<string>
                                 {
                                     Assembly.GetExecutingAssembly().GetPath(),
                                     Assembly.GetCallingAssembly().GetPath(),
                                 };

            if (modelType != null)
            {
                assemblies.Add(modelType.Assembly.GetPath());
            }

            assemblies.AddRange(
                Assembly.GetCallingAssembly().GetReferencedAssemblies().Where(an => an.EscapedCodeBase != null).Select(
                    an => an.GetPath()));
            return assemblies;
        }

        private static RazorTemplateEngine CreateRazorTemplateEngine(Type baseType)
        {
            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
                           {
                               DefaultBaseClass = baseType.FullName,
                               DefaultClassName = "SimpleTemplate",
                               DefaultNamespace = "Simple.Web.Razor"
                           };
            var engine = new RazorTemplateEngine(host);
            return engine;
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
                        modelType = FindModelType(line);
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

        private static readonly HashSet<Assembly> KnownGoodAssemblies = new HashSet<Assembly>();

        private static Type FindModelType(string line)
        {
            string typeName = line.Trim().Replace("@model ", "");
            var modelType = Type.GetType(typeName);

            if (modelType == null)
            {
                foreach (var assembly in KnownGoodAssemblies)
                {
                    if ((modelType = assembly.GetType(typeName)) != null)
                    {
                        break;
                    }
                }
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if ((modelType = assembly.GetType(typeName)) != null)
                    {
                        KnownGoodAssemblies.Add(assembly);
                        break;
                    }
                }
            }
            return modelType;
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
