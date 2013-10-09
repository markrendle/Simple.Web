namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal class RazorTypeBuilderContext
    {
        private static readonly string[] ExcludedReferencesOnMono =
            new[] { "System", "System.Core", "Microsoft.CSharp", "mscorlib" };

        private readonly CompilerParameters _compilerParameters;
        private readonly string _className;
        private List<Assembly> _defaultAssemblies;

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

        private IEnumerable<Assembly> DefaultAssemblies
        {
            get
            {
                if (_defaultAssemblies == null)
                {
                    var folderAssemblies = ScanFolderForAssemblies();
                    _defaultAssemblies = SimpleRazorConfiguration.NamespaceImports.Where(ni => ni.Value != null)
                        .Select(ni => ni.Value)
                        .Concat(folderAssemblies)
                        .Where(IsValidReference)
                        .Where(an => !an.IsDynamic)
                        .GroupBy(an => an.FullName)
                        .Select(an => an.First())
                        .ToList();
                }
                return _defaultAssemblies;
            }
        }

        private static IEnumerable<Assembly> ScanFolderForAssemblies()
        {
            var uri = new Uri(Assembly.GetCallingAssembly().EscapedCodeBase);
            var file = new FileInfo(uri.LocalPath);
            var currentDirectory = file.Directory;
            var assemblyFiles = currentDirectory.GetFiles("*.dll");
            var assemblies = assemblyFiles.Select(y => Assembly.LoadFile(y.FullName));
            return assemblies;
        }

        public string ClassName { get { return _className; } }

        public CompilerParameters GetCompilerParameters()
        {
            var assemblieLocations = DefaultAssemblies.Select(y => y.Location).ToArray();
            _compilerParameters.ReferencedAssemblies.AddRange(assemblieLocations);
            return _compilerParameters;
        }

        private static bool IsValidReference(Assembly assembly)
        {
            return ((Type.GetType("Mono.Runtime") == null) || !ExcludedReferencesOnMono.Any(assembly.Location.Contains));
        }
    }
}