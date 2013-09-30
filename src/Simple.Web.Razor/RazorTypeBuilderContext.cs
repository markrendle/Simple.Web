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
        private static readonly string[] ExcludedReferencesOnMono = { "System", "System.Core", "Microsoft.CSharp", "mscorlib" };

        private static readonly Func<Assembly, bool> IsValidReference =
            an => ((Type.GetType("Mono.Runtime") == null) || !ExcludedReferencesOnMono.Any(an.Location.Contains));

        private readonly string _className;
        private readonly CompilerParameters _compilerParameters;
        private readonly List<Assembly> _declarationAssemblies;
        private Type _handler;
        private Type _model;

        public RazorTypeBuilderContext()
        {
            _className = string.Format("{0}_{1}", SimpleRazorConfiguration.ClassPrefix, Guid.NewGuid().ToString("N"));

            _compilerParameters = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    TreatWarningsAsErrors = false,
                    OutputAssembly = Path.Combine(Path.GetTempPath(), string.Format("{0}.dll", _className)),
                    MainClass = _className
                };

            _declarationAssemblies = new List<Assembly>();
        }

        public string ClassName
        {
            get { return _className; }
        }

        public CompilerParameters GetCompilerParameters()
        {
            var assemblieLocations =
                TypeResolver.DefaultAssemblies.Union(_declarationAssemblies)
                            .Where(IsValidReference)
                            .GroupBy(an => an.Location)
                            .Select(an => an.First().Location)
                            .ToArray();

            _compilerParameters.ReferencedAssemblies.AddRange(assemblieLocations);

            return _compilerParameters;
        }

        public void SetHandler(Type handler)
        {
            _handler = handler;
            AddAssemblyForType(_handler);
        }

        public void SetModel(Type model)
        {
            _model = model;
            AddAssemblyForType(_model);
        }

        private void AddAssemblyForType(Type model)
        {
            _declarationAssemblies.Add(model.Assembly);
            if (model.IsGenericType)
            {
                model.GetGenericArguments().ToList().ForEach(AddAssemblyForType);
            }
        }
    }
}