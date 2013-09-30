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
        private static readonly IDictionary<String, String> CompilerProperties = new Dictionary<String, String>
            {
                { "CompilerVersion", "v4.0" }
            };

        private static readonly Dictionary<string, Action<string, RazorTypeBuilderContext>> DirectiveHandlers =
            new Dictionary<string, Action<string, RazorTypeBuilderContext>> { { "@handler", ReadHandler }, { "@model", ReadModel } };

        public Type CreateType(StreamReader reader)
        {
            return CreateTypeImpl(reader);
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

        private static RazorTypeBuilderContext CreateContext(StreamReader reader)
        {
            var context = new RazorTypeBuilderContext();
            var lines = GetLines(reader);
            PreProcess(lines, context);
            return context;
        }

        private static RazorTemplateEngine CreateRazorTemplateEngine()
        {
            var language = new CSharpRazorCodeLanguage();
            var host = new SimpleRazorEngineHost(language);
            var engine = new RazorTemplateEngine(host);

            return engine;
        }

        private static Type CreateTypeImpl(StreamReader reader)
        {
            var context = CreateContext(reader);

            var engine = CreateRazorTemplateEngine();
            var razorResult = engine.GenerateCode(reader, context.ClassName, engine.Host.DefaultNamespace, null);
            var viewType = CompileView(razorResult, context.GetCompilerParameters());

            return viewType;
        }

        private static List<string> GetLines(StreamReader reader)
        {
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            reader.BaseStream.Position = 0;
            return lines;
        }

        private static void PreProcess(List<string> lines, RazorTypeBuilderContext context)
        {
            lines.ForEach(y =>
                          {
                              var directive = y.Split(' ')[0];
                              if (DirectiveHandlers.Keys.Contains(directive))
                              {
                                  DirectiveHandlers[directive](y, context);
                              }
                          });
        }

        private static void ReadHandler(string line, RazorTypeBuilderContext context)
        {
            var type = TypeHelper.FindTypeFromRazorLine(line, "@handler");
            context.SetHandler(type);
        }

        private static void ReadModel(string line, RazorTypeBuilderContext context)
        {
            var type = TypeHelper.FindTypeFromRazorLine(line, "@model");
            context.SetModel(type);
        }
    }

    internal static class TypeHelper
    {
        private static readonly TypeResolver TypeResolver = new TypeResolver();

        public static Type FindTypeFromRazorLine(string line, string directive)
        {
            string typeName = line.Replace(directive, string.Empty).Trim();
            return TypeResolver.FindType(typeName);
        }
    }
}