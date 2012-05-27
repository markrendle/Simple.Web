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
            Path.GetDirectoryName(typeof(RazorViews).Assembly.GetPath()).Regex(@"\\bin\\?$", string.Empty);

        public static void Initialize()
        {
            ClearCaches();

            const string directory = "Views";

            var viewsDirectory = Path.Combine(AppRoot, directory);

            Console.WriteLine(viewsDirectory);
            Trace.WriteLine(viewsDirectory);

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

        public Type GetViewType(Type handlerType, Type modelType)
        {
            InitializeIfNot();

            Type type;

            if (TryGetCombinedType(handlerType, modelType, out type)) return type;

            if (TryGetHandlerType(handlerType, modelType, out type)) return type;

            if (handlerType != null)
            {
                if (TryGetHandlerType(handlerType.GetInterfaces(), out type)) return type;
            }

            if (TryGetModelType(handlerType, modelType, out type)) return type;

            if (modelType != null)
            {
                if (TryGetModelType(modelType.GetInterfaces(), out type)) return type;
            }

            throw new ViewNotFoundException(handlerType, modelType);
        }

        private static bool TryGetModelType(Type handlerType, Type modelType, out Type type)
        {
            if (modelType == null)
            {
                type = null;
                return false;
            }
            if (AmbiguousModelTypes.Contains(modelType))
            {
                throw new AmbiguousViewException(handlerType, modelType);
            }
            if (ModelTypeCache.ContainsKey(modelType))
            {
                {
                    type = ModelTypeCache[modelType];
                    return true;
                }
            }

            return TryGetModelType(handlerType, modelType.BaseType, out type);
        }

        private static bool TryGetModelType(Type[] modelTypeInterfaces, out Type type)
        {
            if (modelTypeInterfaces == null || modelTypeInterfaces.Length == 0)
            {
                type = null;
                return false;
            }

            var q = from @interface in modelTypeInterfaces
                    where ModelTypeCache.ContainsKey(@interface)
                    select ModelTypeCache[@interface];
            var list = q.ToList();
            if (list.Count == 1)
            {
                type = list[0];
                return true;
            }

            type = null;
            return false;
        }

        private static bool TryGetHandlerType(Type handlerType, Type modelType, out Type type)
        {
            if (handlerType == null)
            {
                type = null;
                return false;
            }
            if (AmbiguousHandlerTypes.Contains(handlerType))
            {
                throw new AmbiguousViewException(handlerType, modelType);
            }
            if (HandlerTypeCache.ContainsKey(handlerType))
            {
                {
                    type = HandlerTypeCache[handlerType];
                    return true;
                }
            }

            return TryGetHandlerType(handlerType.BaseType, modelType, out type);
        }
        
        private static bool TryGetHandlerType(Type[] handlerTypeInterfaces, out Type type)
        {
            if (handlerTypeInterfaces == null || handlerTypeInterfaces.Length == 0)
            {
                type = null;
                return false;
            }

            var q = from @interface in handlerTypeInterfaces
                       where HandlerTypeCache.ContainsKey(@interface)
                       select HandlerTypeCache[@interface];
            var list = q.ToList();
            if (list.Count == 1)
            {
                type = list[0];
                return true;
            }

            type = null;
            return false;
        }

        private static bool TryGetCombinedType(Type handlerType, Type modelType, out Type type)
        {
            if (handlerType != null && modelType != null)
            {
                var key = Tuple.Create(handlerType, modelType);

                if (AmbiguousHandlerAndModelTypes.Contains(key))
                {
                    throw new AmbiguousViewException(handlerType, modelType);
                }
                if (HandlerAndModelTypeCache.ContainsKey(key))
                {
                    {
                        type = HandlerAndModelTypeCache[key];
                        return true;
                    }
                }

                if (handlerType.BaseType != null)
                {
                    if (TryGetCombinedType(handlerType.BaseType, modelType, out type)) return true;
                }

                if (modelType.BaseType != null)
                {
                    if (TryGetCombinedType(handlerType, modelType.BaseType, out type)) return true;
                }
            }

            type = null;
            return false;
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