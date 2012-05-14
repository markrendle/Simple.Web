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
        private static readonly Dictionary<Tuple<Type,Type>, Type> HandlerAndModelTypeCache = new Dictionary<Tuple<Type, Type>, Type>();
        private static readonly Dictionary<Type, Type> HandlerTypeCache = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, Type> ModelTypeCache = new Dictionary<Type, Type>();
        private static readonly HashSet<Tuple<Type,Type>> AmbiguousHandlerAndModelTypes = new HashSet<Tuple<Type, Type>>();
        private static readonly HashSet<Type> AmbiguousHandlerTypes = new HashSet<Type>();
        private static readonly HashSet<Type> AmbiguousModelTypes = new HashSet<Type>();
        private static readonly RazorTypeBuilder RazorTypeBuilder = new RazorTypeBuilder();

        private static readonly string AppRoot =
            Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetPath())).Regex(@"\\bin\\?$",
                                                                                                          string.Empty);

        public static void Initialize()
        {
            ClearCaches();

            const string directory = "Views";

            var viewsDirectory = Path.Combine(AppRoot, directory);

            if (!Directory.Exists(viewsDirectory))
            {
                return;
            }

            FindViews(viewsDirectory);
        }

        private static void ClearCaches()
        {
            ViewPathCache.Clear();
            ModelTypeCache.Clear();
            HandlerTypeCache.Clear();
            HandlerAndModelTypeCache.Clear();
            AmbiguousModelTypes.Clear();
            AmbiguousHandlerTypes.Clear();
            AmbiguousHandlerAndModelTypes.Clear();
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
                            if (!CacheViewTypeByHandlerAndModelType(type))
                            {
                                CacheViewTypeByModelType(type);
                                CacheViewTypeByHandlerType(type);
                            }
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

            if (!IsGeneric(baseType, typeof(SimpleTemplateModelBase<>), typeof(SimpleTemplateHandlerModelBase<,>))) return;

            int modelTypeIndex = baseType.GetGenericArguments().Length - 1;
            var modelType = baseType.GetGenericArguments()[modelTypeIndex];
            if (AmbiguousModelTypes.Contains(modelType)) return;
            if (ModelTypeCache.ContainsKey(modelType))
            {
                AmbiguousModelTypes.Add(modelType);
                ModelTypeCache.Remove(modelType);
            }
            else
            {
                ModelTypeCache.Add(modelType, type);
            }
        }

        private static void CacheViewTypeByHandlerType(Type type)
        {
            var baseType = type.BaseType;

            if (!IsGeneric(baseType, typeof(SimpleTemplateHandlerBase<>), typeof(SimpleTemplateHandlerModelBase<,>))) return;

            var handlerType = baseType.GetGenericArguments()[0];
            if (AmbiguousModelTypes.Contains(handlerType)) return;
            if (HandlerTypeCache.ContainsKey(handlerType))
            {
                AmbiguousHandlerTypes.Add(handlerType);
                HandlerTypeCache.Remove(handlerType);
            }
            else
            {
                HandlerTypeCache.Add(handlerType, type);
            }
        }

        private static bool CacheViewTypeByHandlerAndModelType(Type type)
        {
            var baseType = type.BaseType;

            if (!IsGeneric(baseType, typeof(SimpleTemplateHandlerModelBase<,>))) return false;

            var genericArgs = baseType.GetGenericArguments();
            var key = Tuple.Create(genericArgs[0], genericArgs[1]);
            if (AmbiguousHandlerAndModelTypes.Contains(key)) return false;
            if (HandlerAndModelTypeCache.ContainsKey(key))
            {
                AmbiguousHandlerAndModelTypes.Add(key);
                HandlerAndModelTypeCache.Remove(key);
            }
            else
            {
                HandlerAndModelTypeCache.Add(key, type);
            }
            return true;
        }

        private static bool IsGeneric(Type baseType, params Type[] validTypes)
        {
            if (baseType == null) return false;
            if (!baseType.IsGenericType) return false;
            var genericType = baseType.GetGenericTypeDefinition();
            return validTypes.Contains(genericType);
        }

        public Type GetViewTypeForModelType(Type modelType)
        {
            InitializeIfNot();

            if (AmbiguousModelTypes.Contains(modelType))
            {
                throw new AmbiguousViewException(null, modelType);
            }
            try
            {
                return ModelTypeCache[modelType];
            }
            catch (KeyNotFoundException)
            {
                throw new ViewNotFoundException(null, modelType);
            }
        }

        public Type GetViewTypeForHandlerType(Type handlerType)
        {
            InitializeIfNot();

            if (AmbiguousHandlerTypes.Contains(handlerType))
            {
                throw new AmbiguousViewException(handlerType, null);
            }
            try
            {
                return HandlerTypeCache[handlerType];
            }
            catch (KeyNotFoundException)
            {
                throw new ViewNotFoundException(handlerType, null);
            }
        }

        public Type GetViewTypeForHandlerAndModelType(Type handlerType, Type modelType)
        {
            InitializeIfNot();

            if (handlerType != null && modelType != null)
            {
                var key = Tuple.Create(handlerType, modelType);

                if (AmbiguousHandlerAndModelTypes.Contains(key))
                {
                    throw new AmbiguousViewException(handlerType, modelType);
                }
                if (HandlerAndModelTypeCache.ContainsKey(key))
                {
                    return HandlerAndModelTypeCache[key];
                }
            }

            if (handlerType != null)
            {
                if (AmbiguousHandlerTypes.Contains(handlerType))
                {
                    throw new AmbiguousViewException(handlerType, modelType);
                }
                if (HandlerTypeCache.ContainsKey(handlerType))
                {
                    return HandlerTypeCache[handlerType];
                }
            }

            if (modelType != null)
            {
                if (AmbiguousModelTypes.Contains(modelType))
                {
                    throw new AmbiguousViewException(handlerType, modelType);
                }
                if (ModelTypeCache.ContainsKey(modelType))
                {
                    return ModelTypeCache[modelType];
                }
            }
            
            throw new ViewNotFoundException(handlerType, modelType);
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
            var key = Path.Combine("Views", viewPath);
            if (!ViewPathCache.ContainsKey(key))
            {
                Initialize();
            }
            return ViewPathCache[key];
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