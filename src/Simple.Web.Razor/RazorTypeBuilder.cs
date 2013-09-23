using System.Web.Razor.Text;

namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.Razor;

    using Microsoft.CSharp;

    using Simple.Web.Razor.Engine;

    internal class RazorTypeBuilderContext
    {
        private static readonly string[] ExcludedReferencesOnMono =
            new[] { "System", "System.Core", "Microsoft.CSharp", "mscorlib" };

        private static readonly Func<Assembly, bool> IsValidReference = an =>
                ((Type.GetType("Mono.Runtime") == null) || !ExcludedReferencesOnMono.Any(an.Location.Contains));

        private Type _model;
        private Type _handler;
        private List<string> _lines;
        private StreamReader _reader;
        private CompilerParameters _compilerParameters;
        private string _className;

        public RazorTypeBuilderContext()
        {
            _className = string.Format("{0}_{1}", SimpleRazorConfiguration.ClassPrefix, Guid.NewGuid().ToString("N"));

            _compilerParameters =
            new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                OutputAssembly = Path.Combine(Path.GetTempPath(), string.Format("{0}.dll", _className)),
                MainClass = _className
            };
        }

        public string ClassName {get { return _className; }}

        public void SetModel(Type model)
        {
            _model = model;
        }

        public void SetHandler(Type handler)
        {
            _handler = handler;
        }

        public void SetLines(List<string> lines)
        {
            _lines = lines;
        }

        public void SetReader(StreamReader reader)
        {
            _reader = reader;
        }

        public CompilerParameters GetCompilerParameters()
        {
            var declarationAssemblies = FindDeclarationAssemblies();



            _compilerParameters.ReferencedAssemblies.AddRange(
                TypeResolver.DefaultAssemblies
                .Union(declarationAssemblies)
                .Where(an => IsValidReference(an))
                .Select(an => an.Location).ToArray());

            return _compilerParameters;
        }

        private IEnumerable<Assembly> FindDeclarationAssemblies()
        {
            return GetNormalizedAssemblies(
                new Type[] { _model, _handler }
                    .Concat(_model != null && _model.IsGenericType ? _model.GetGenericArguments() : new Type[0]).ToArray())
                    .GroupBy(an => an.Location)
                    .Select(an => an.First())
                    .ToArray();
        }

        private static IEnumerable<Assembly> GetNormalizedAssemblies(params Type[] types)
        {
            return from type in types where type != null select type.Assembly;
        }

    }

    internal class RazorTypeBuilder
    {
        private static Dictionary<string, Func<string, RazorTypeBuilderContext, RazorTypeBuilderContext>>
            _directiveHandlers =
                new Dictionary<string, Func<string, RazorTypeBuilderContext, RazorTypeBuilderContext>>()
                {
                    {"@handler", ReadHandler},
                    {"@model", ReadModel}
                };

        private static RazorTypeBuilderContext ReadModel(string line, RazorTypeBuilderContext context)
        {
            var type = TypeHelper.FindTypeFromRazorLine(line, "@model");
            context.SetModel(type);
            return context;
        }

        private static RazorTypeBuilderContext ReadHandler(string line, RazorTypeBuilderContext context)
        {
            var type = TypeHelper.FindTypeFromRazorLine(line, "@handler");
            context.SetHandler(type);
            return context;
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

        private static RazorTypeBuilderContext CreateContext(StreamReader reader)
        {
            var context = new RazorTypeBuilderContext();
            context.SetReader(reader);
            var lines = GetLines(reader);
            context.SetLines(lines);

            PreProcess(lines, context);

            return context;
        }

        private static void PreProcess(List<string> lines, RazorTypeBuilderContext context)
        {
            lines.ForEach(y =>
            {
                var splittedLine = y.Split(' ');
                if (_directiveHandlers.Keys.Contains(splittedLine[0]))
                {
                    _directiveHandlers[splittedLine[0]](y, context);
                }
            });
        }

        private static readonly IDictionary<String, String> CompilerProperties =
            new Dictionary<String, String> { { "CompilerVersion", "v4.0" } };

        public Type CreateType(StreamReader reader)
        {
            return CreateTypeImpl(reader);
        }

        private static Type CreateTypeImpl(StreamReader reader)
        {
            var context = CreateContext(reader);

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

    internal static class TypeHelper
    {
        private static readonly TypeResolver TypeResolver = new TypeResolver();

        internal static Type ExtractType(ref TextReader reader, string directive)
        {
            Type type = null;

            using (var writer = new StringWriter())
            {
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (type == null && line != null && line.Trim().StartsWith(directive))
                    {
                        type = FindTypeFromRazorLine(line, directive);
                    }

                    writer.WriteLine(line);
                }


                using (Interlocked.CompareExchange(ref reader, new StringReader(writer.ToString()), reader))
                {
                }
            }

            return type;
        }

        public static Type FindTypeFromRazorLine(string line, string directive)
        {
            string typeName = line.Replace(directive, string.Empty).Trim();
            return TypeHelper.TypeResolver.FindType(typeName);
        }
    }
}
