namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class RazorViews
    {
        private static readonly object LockObject = new object();
        private static volatile bool _initialized;
        private static readonly Dictionary<string, Type> ViewPathCache = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, Type> ViewTypeCache = new Dictionary<Type, Type>();
        private static readonly HashSet<Type> AmbiguousModelTypes = new HashSet<Type>(); 
        private static readonly RazorTypeBuilder RazorTypeBuilder = new RazorTypeBuilder();

        private static readonly string AppRoot =
            Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetPath())).Regex(@"\\bin\\?$",
                                                                                                          string.Empty);

        public static void Initialize()
        {
            ViewPathCache.Clear();
            ViewTypeCache.Clear();
            AmbiguousModelTypes.Clear();

            const string directory = "Views";

            var viewsDirectory = Path.Combine(AppRoot, directory);

            if (!Directory.Exists(viewsDirectory))
            {
                return;
            }

            FindViews(viewsDirectory);
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
                            var token = Path.GetDirectoryName(file).Replace(AppRoot + Path.DirectorySeparatorChar, "");
                            ViewPathCache.Add(Path.Combine(token, Path.GetFileNameWithoutExtension(file)), type);
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
            InitializeIfNot();

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

        private static void InitializeIfNot()
        {
            if (!_initialized)
            {
                lock (LockObject)
                {
                    if (!_initialized)
                    {
                        Initialize();
                        _initialized = true;
                    }
                }
            }
        }

        public Type GetViewType(string viewPath)
        {
            if (viewPath == null) throw new ArgumentNullException("viewPath");
            InitializeIfNot();
            if (viewPath.EndsWith(".cshtml")) viewPath = Path.GetFileNameWithoutExtension(viewPath);
            return ViewPathCache[Path.Combine("Views", viewPath)];
        }
    }

    internal static class RegexEx
    {
        public static string Regex(this string target, string pattern, string replaceWith)
        {
            return new Regex(pattern).Replace(target, replaceWith);
        }
    }
}