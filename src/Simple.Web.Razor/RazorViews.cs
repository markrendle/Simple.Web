namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Simple.Web.Helpers;

    internal class RazorViews
    {
        private const string AppSettings_CacheEnabled = "ViewCache:Enabled";
        private const string AppSettings_Path = "ViewPath:Value";
        private const string AppSettings_RecursiveDiscovery = "ViewRecursiveDiscovery:Enabled";
        private const string DefaultViewPath = "Views";
        private const string FileExtension = "cshtml";

        private static readonly HashSet<Tuple<string, string>> AmbiguousHandlerAndModelTypes = new HashSet<Tuple<string, string>>();
        private static readonly HashSet<string> AmbiguousHandlerTypes = new HashSet<string>();
        private static readonly HashSet<string> AmbiguousModelTypes = new HashSet<string>();

        private static readonly string AppRoot = AssemblyAppRoot(typeof(RazorViews).Assembly.GetPath());

        private static readonly Func<string, string> FileFormatter = prefix => string.Format("{0}.{1}", prefix, FileExtension);

        private static readonly Func<string, string> FileTokenizer =
            file =>
            Path.Combine(
                file.StartsWith("~/")
                    ? Path.GetDirectoryName(file.Substring(2))
                    : Path.GetDirectoryName(file).Replace(AppRoot + Path.DirectorySeparatorChar, ""),
                Path.GetFileNameWithoutExtension(file));

        private static readonly Dictionary<Tuple<string, string>, string> HandlerAndModelTypeCache =
            new Dictionary<Tuple<string, string>, string>();

        private static readonly Dictionary<string, string> HandlerTypeCache = new Dictionary<string, string>();
        private static readonly bool IsCacheEnabled = ParseBooleanAppSettings(AppSettings_CacheEnabled);
        private static readonly object LockObject = new object();
        private static readonly Dictionary<string, string> ModelTypeCache = new Dictionary<string, string>();
        private static readonly RazorTypeBuilder RazorTypeBuilder = new RazorTypeBuilder();
        private static readonly Dictionary<string, Func<Type>> TypeCache = new Dictionary<string, Func<Type>>();

        private static readonly Dictionary<string, string> ViewPathCache = new Dictionary<string, string>();
        private static volatile bool _initialized;

        public Type GetViewType(Type handlerType, Type modelType)
        {
            InitializeIfNot();

            Type type;

            if (TryGetCombinedType(handlerType, modelType, out type))
            {
                return type;
            }

            if (TryGetHandlerType(handlerType, modelType, out type))
            {
                return type;
            }

            if (handlerType != null)
            {
                if (TryGetHandlerType(handlerType.GetInterfaces(), out type))
                {
                    return type;
                }
            }

            if (TryGetModelType(handlerType, modelType, out type))
            {
                return type;
            }

            if (modelType != null)
            {
                if (TryGetModelType(modelType.GetInterfaces(), out type))
                {
                    return type;
                }
            }

            throw new ViewNotFoundException(handlerType, modelType);
        }

        public Type GetViewType(string viewPath)
        {
            if (viewPath == null)
            {
                throw new ArgumentNullException("viewPath");
            }

            InitializeIfNot();

            var key = FileTokenizer(viewPath);

            if (!ViewPathCache.ContainsKey(key))
            {
                return GenerateViewType(FileFormatter(key));
            }

            return TypeCache[ViewPathCache[key]].Invoke();
        }

        public static string AssemblyAppRoot(string typePath)
        {
            return Path.GetDirectoryName(typePath).Regex(@"(\\|/)bin(\\|/)?([Dd]ebug|[Rr]elease)?$", string.Empty);
        }

        public static void Initialize()
        {
            ClearCaches();

            string searchPath = ParseStringAppSettings(AppSettings_Path, DefaultViewPath).Trim('/', '\\');
            bool recursiveDiscovery = ParseBooleanAppSettings(AppSettings_RecursiveDiscovery);

            var viewsDirectory = Path.Combine(AppRoot, searchPath);

            if (!Directory.Exists(viewsDirectory))
            {
                return;
            }

            FindViews(viewsDirectory, recursiveDiscovery);
        }

        private static void CachePageType(Type type, string file)
        {
            var token = FileTokenizer(file);

            TypeCache.Add(type.FullName, () => RuntimeTypeCheck(type));

            if (ViewPathCache.ContainsKey(token))
            {
                if (!IsCacheEnabled)
                {
                    ClearCaches(ViewPathCache[token]);
                    ViewPathCache[token] = type.FullName;
                }
                else
                {
                    throw new ArgumentException(string.Format("Unable to add duplicate view type for '{0}'.", file), "file");
                }
            }
            else
            {
                ViewPathCache.Add(token, type.FullName);
            }

            if (!CacheViewTypeByHandlerAndModelType(type))
            {
                CacheViewTypeByModelType(type);
                CacheViewTypeByHandlerType(type);
            }
        }

        private static bool CacheViewTypeByHandlerAndModelType(Type type)
        {
            var baseType = type.BaseType;

            if (!IsGeneric(baseType, typeof(SimpleTemplateHandlerModelBase<,>)))
            {
                return false;
            }

            var genericArgs = baseType.GetGenericArguments();

            var key = Tuple.Create(genericArgs[0].FullName, genericArgs[1].FullName);

            if (AmbiguousHandlerAndModelTypes.Contains(key))
            {
                return false;
            }

            if (HandlerAndModelTypeCache.ContainsKey(key))
            {
                AmbiguousHandlerAndModelTypes.Add(key);
                HandlerAndModelTypeCache.Remove(key);
            }
            else
            {
                HandlerAndModelTypeCache.Add(key, type.FullName);
            }

            return true;
        }

        private static void CacheViewTypeByHandlerType(Type type)
        {
            var baseType = type.BaseType;

            if (!IsGeneric(baseType, typeof(SimpleTemplateHandlerBase<>), typeof(SimpleTemplateHandlerModelBase<,>)))
            {
                return;
            }

            var handlerType = baseType.GetGenericArguments()[0];

            if (AmbiguousModelTypes.Contains(handlerType.FullName))
            {
                return;
            }

            if (HandlerTypeCache.ContainsKey(handlerType.FullName))
            {
                AmbiguousHandlerTypes.Add(handlerType.FullName);
                HandlerTypeCache.Remove(handlerType.FullName);
            }
            else
            {
                HandlerTypeCache.Add(handlerType.FullName, type.FullName);
            }
        }

        private static void CacheViewTypeByModelType(Type type)
        {
            var baseType = type.BaseType;

            if (!IsGeneric(baseType, typeof(SimpleTemplateModelBase<>), typeof(SimpleTemplateHandlerModelBase<,>)))
            {
                return;
            }

            int modelTypeIndex = baseType.GetGenericArguments().Length - 1;

            var modelType = baseType.GetGenericArguments()[modelTypeIndex];

            if (AmbiguousModelTypes.Contains(modelType.FullName))
            {
                return;
            }

            if (ModelTypeCache.ContainsKey(modelType.FullName))
            {
                AmbiguousModelTypes.Add(modelType.FullName);
                ModelTypeCache.Remove(modelType.FullName);
            }
            else
            {
                ModelTypeCache.Add(modelType.FullName, type.FullName);
            }
        }

        private static void ClearCaches(string typeName = null)
        {
            ClearTypesIn(ViewPathCache, typeName);
            ClearTypesIn(ModelTypeCache, typeName);
            ClearTypesIn(ViewPathCache, typeName);
            ClearTypesIn(ModelTypeCache, typeName);
            ClearTypesIn(HandlerTypeCache, typeName);
            ClearTypesIn(HandlerAndModelTypeCache, typeName);

            AmbiguousModelTypes.RemoveWhere(x => typeName == null || x.Equals(typeName));
            AmbiguousHandlerTypes.RemoveWhere(x => typeName == null || x.Equals(typeName));
            AmbiguousHandlerAndModelTypes.RemoveWhere(x => typeName == null || x.Equals(typeName));

            if (typeName == null)
            {
                TypeCache.Clear();
            }
            else
            {
                TypeCache.Remove(typeName);
            }
        }

        private static void ClearTypesIn(IDictionary<string, string> dictionary, string typeName = null)
        {
            if (String.IsNullOrWhiteSpace(typeName))
            {
                dictionary.Clear();
            }
            else
            {
                KeyValuePair<string, string>? pair = dictionary.FirstOrDefault(x => x.Value.Equals(typeName));

                if (pair != null && pair.Value.Key != null)
                {
                    dictionary.Remove(pair.Value.Key);
                }
            }
        }

        private static void ClearTypesIn(IDictionary<Tuple<string, string>, string> dictionary, string typeName = null)
        {
            if (String.IsNullOrWhiteSpace(typeName))
            {
                dictionary.Clear();
            }
            else
            {
                KeyValuePair<Tuple<string, string>, string>? pair = HandlerAndModelTypeCache.FirstOrDefault(x => x.Value.Equals(typeName));

                if (pair != null && pair.Value.Key != null)
                {
                    HandlerAndModelTypeCache.Remove(pair.Value.Key);
                }
            }
        }

        private static void FindViews(string directory, bool recurse = true)
        {
            foreach (var file in Directory.GetFiles(directory, FileFormatter("*")))
            {
                GenerateViewType(file);
            }

            if (recurse)
            {
                foreach (var subDirectory in Directory.GetDirectories(directory).Select(sub => Path.Combine(directory, sub)))
                {
                    FindViews(subDirectory);
                }
            }
        }

        private static Type GenerateViewType(string pathname)
        {
            using (var reader = new StreamReader(pathname))
            {
                try
                {
                    var type = RazorTypeBuilder.CreateType(reader);

                    if (TypeCache.ContainsKey(type.FullName))
                    {
                        throw new ArgumentException(String.Format("Duplicate generated type '{0}' is not permitted", type.FullName),
                                                    pathname);
                    }

                    CachePageType(type, pathname);

                    return type;
                }
                catch (RazorCompilerException ex)
                {
                    Debug.WriteLine("*** View compile failed for " + pathname + ": " + ex.Message);
                    Trace.TraceError(ex.Message);
                }
            }

            return null;
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

        private static bool IsGeneric(Type baseType, params Type[] validTypes)
        {
            if (baseType == null)
            {
                return false;
            }

            if (!baseType.IsGenericType)
            {
                return false;
            }

            var genericType = baseType.GetGenericTypeDefinition();

            return validTypes.Contains(genericType);
        }

        private static bool ParseBooleanAppSettings(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            bool isEnabled;

            if (!bool.TryParse(value, out isEnabled))
            {
                throw new ConfigurationErrorsException(string.Format("Invalid configuration value for appSetting '{0}'", key));
            }

            return isEnabled;
        }

        private static string ParseStringAppSettings(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (String.IsNullOrWhiteSpace(value))
            {
                if (String.IsNullOrWhiteSpace(defaultValue))
                {
                    throw new ConfigurationErrorsException(string.Format("Invalid configuration value for appSetting '{0}'", key));
                }

                return defaultValue;
            }

            return value;
        }

        private static Type RuntimeTypeCheck(Type type)
        {
            if (!IsCacheEnabled)
            {
                return
                    GenerateViewType(Path.Combine(AppRoot,
                                                  FileFormatter(ViewPathCache.FirstOrDefault(p => p.Value.Equals(type.FullName)).Key)));
            }

            return type;
        }

        private static bool TryGetCombinedType(Type handlerType, Type modelType, out Type type)
        {
            if (handlerType != null && modelType != null)
            {
                var key = Tuple.Create(handlerType.FullName, modelType.FullName);

                if (AmbiguousHandlerAndModelTypes.Contains(key))
                {
                    throw new AmbiguousViewException(handlerType, modelType);
                }

                if (HandlerAndModelTypeCache.ContainsKey(key))
                {
                    type = TypeCache[HandlerAndModelTypeCache[key]].Invoke();
                    return true;
                }

                if (handlerType.BaseType != null)
                {
                    if (TryGetCombinedType(handlerType.BaseType, modelType, out type))
                    {
                        return true;
                    }
                }

                if (modelType.BaseType != null)
                {
                    if (TryGetCombinedType(handlerType, modelType.BaseType, out type))
                    {
                        return true;
                    }
                }
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

            if (AmbiguousHandlerTypes.Contains(handlerType.FullName))
            {
                throw new AmbiguousViewException(handlerType, modelType);
            }

            if (HandlerTypeCache.ContainsKey(handlerType.FullName))
            {
                {
                    type = TypeCache[HandlerTypeCache[handlerType.FullName]].Invoke();
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
                    where HandlerTypeCache.ContainsKey(@interface.FullName)
                    select HandlerTypeCache[@interface.FullName];

            var list = q.ToList();

            if (list.Count == 1)
            {
                type = TypeCache[list[0]].Invoke();
                return true;
            }

            type = null;

            return false;
        }

        private static bool TryGetModelType(Type handlerType, Type modelType, out Type type)
        {
            if (modelType == null)
            {
                type = null;
                return false;
            }

            if (AmbiguousModelTypes.Contains(modelType.FullName))
            {
                throw new AmbiguousViewException(handlerType, modelType);
            }

            if (ModelTypeCache.ContainsKey(modelType.FullName))
            {
                type = TypeCache[ModelTypeCache[modelType.FullName]].Invoke();
                return true;
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
                    where ModelTypeCache.ContainsKey(@interface.FullName)
                    select ModelTypeCache[@interface.FullName];
            var list = q.ToList();
            if (list.Count == 1)
            {
                type = TypeCache[list[0]].Invoke();
                return true;
            }

            type = null;
            return false;
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