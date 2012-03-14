namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal class RazorViews
    {
        private static readonly object LockObject = new object();
        private static volatile bool _initialized;
        private static readonly Dictionary<string, Type> ViewPathCache = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, Type> ViewTypeCache = new Dictionary<Type, Type>();
        private static readonly HashSet<Type> AmbiguousModelTypes = new HashSet<Type>(); 
        private static readonly RazorTypeBuilder RazorTypeBuilder = new RazorTypeBuilder();

        public static void Initialize()
        {
            ViewPathCache.Clear();
            ViewTypeCache.Clear();
            AmbiguousModelTypes.Clear();

            const string directory = "Views";
            if (!Directory.Exists(directory))
            {
                return;
            }

            FindViews(directory);
        }

        private static void FindViews(string directory)
        {
            foreach (var file in Directory.GetFiles(directory, "*.cshtml"))
            {
                using (var reader = new StreamReader(file))
                {
                    try
                    {
                        var type = RazorTypeBuilder.CreateType(reader);
                        if (type != null)
                        {
                            ViewPathCache.Add(file, type);
                            CacheViewTypeByModelType(type);
                        }
                    }
                    catch (RazorCompilerException ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
            }

            foreach (var subDirectory in Directory.GetDirectories(directory).Select(sub => Path.Combine(directory, sub)))
            {
                FindViews(subDirectory);
            }
        }

        private static void CacheViewTypeByModelType(Type type)
        {
            var baseType = type.BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                var modelType = baseType.GetGenericArguments()[0];
                if (AmbiguousModelTypes.Contains(modelType)) return;
                if (ViewTypeCache.ContainsKey(modelType))
                {
                    AmbiguousModelTypes.Add(modelType);
                    ViewTypeCache.Remove(modelType);
                }
                else
                {
                    ViewTypeCache.Add(modelType, type);
                }
            }
        }

        public Type GetViewTypeForModelType(Type modelType)
        {
            if (!_initialized)
            {
                lock (LockObject)
                {
                    if (!_initialized)
                    {
                        Initialize();
                    }
                }
            }

            if (AmbiguousModelTypes.Contains(modelType))
            {
                throw new AmbiguousViewException(modelType);
            }
            try
            {
                return ViewTypeCache[modelType];
            }
            catch (KeyNotFoundException)
            {
                throw new ViewNotFoundException(modelType);
            }
        }
    }

    public class ViewNotFoundException : Exception
    {
        private readonly Type _modelType;

        public ViewNotFoundException(Type modelType) : base(string.Format("No View present for Model type {0}", modelType.Name))
        {
            _modelType = modelType;
        }

        public Type ModelType
        {
            get { return _modelType; }
        }
    }

    public class AmbiguousViewException : Exception
    {
        private readonly Type _modelType;

        public AmbiguousViewException(Type modelType) : base(string.Format("More than one View present for Model type {0}", modelType.Name))
        {
            _modelType = modelType;
        }

        public Type ModelType
        {
            get { return _modelType; }
        }
    }
}