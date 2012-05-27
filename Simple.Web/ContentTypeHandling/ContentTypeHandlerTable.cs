namespace Simple.Web.ContentTypeHandling
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class ContentTypeHandlerTable
    {
        private static readonly object InitLock = new object();
        private static readonly HashSet<string> UnsupportedMediaTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly List<Tuple<Regex, Func<IContentTypeHandler>>> WildcardContentTypeHandlerFunctions =
            new List<Tuple<Regex, Func<IContentTypeHandler>>>();
        private static readonly ConcurrentDictionary<string, Func<IContentTypeHandler>> ContentTypeHandlerFunctions =
            new ConcurrentDictionary<string, Func<IContentTypeHandler>>();

        public IContentTypeHandler GetContentTypeHandler(string contentType)
        {
            EnsureTableIsPopulated();

            var handler = GetContentTypeHandlerImpl(contentType);

            if (handler == null) throw new UnsupportedMediaTypeException(contentType);

            return handler;
        }

        public IContentTypeHandler GetContentTypeHandler(IList<string> contentTypes, out string matchedType)
        {
            EnsureTableIsPopulated();

            for (int i = 0; i < contentTypes.Count; i++)
            {
                var handler = GetContentTypeHandlerImpl(contentTypes[i]);
                if (handler != null)
                {
                    matchedType = contentTypes[i];
                    return handler;
                }
            }

            throw new UnsupportedMediaTypeException(contentTypes);
        }

        private static IContentTypeHandler GetContentTypeHandlerImpl(string contentType)
        {
            if (UnsupportedMediaTypes.Contains(contentType)) return null;

            Func<IContentTypeHandler> func;
            if (ContentTypeHandlerFunctions.TryGetValue(contentType, out func))
            {
                return func();
            }

            var wildcard = WildcardContentTypeHandlerFunctions.FirstOrDefault(t => t.Item1.IsMatch(contentType));
            if (wildcard != null)
            {
                // Cache the specific content type for faster resolution next time.
                ContentTypeHandlerFunctions.TryAdd(contentType, wildcard.Item2);
                return wildcard.Item2();
            }

            UnsupportedMediaTypes.Add(contentType);
            return null;
        }

        private static void EnsureTableIsPopulated()
        {
            if (ContentTypeHandlerFunctions.Count == 0)
            {
                lock (InitLock)
                {
                    if (ContentTypeHandlerFunctions.Count == 0)
                    {
                        PopulateContentTypeHandlerFunctions();
                    }
                }
            }
        }

        private static void PopulateContentTypeHandlerFunctions()
        {
            foreach (var exportedType in ExportedTypeHelper.FromCurrentAppDomain(TypeIsContentTypeHandler))
            {
                AddContentTypeHandler(exportedType);
            }

            AddContentTypeHandler(typeof(FormDeserializer));
        }

        private static void AddContentTypeHandler(Type exportedType)
        {
            var contentTypes = Attribute.GetCustomAttributes(exportedType, typeof (ContentTypesAttribute))
                .Cast<ContentTypesAttribute>()
                .SelectMany(contentTypesAttribute => contentTypesAttribute.ContentTypes);

            Func<IContentTypeHandler> creator = () => Activator.CreateInstance(exportedType) as IContentTypeHandler;
            foreach (var contentType in contentTypes)
            {
                if (contentType.Contains("*"))
                {
                    var expression = Regex.Escape(contentType).Replace(@"\*", ".*?");
                    WildcardContentTypeHandlerFunctions.Add(Tuple.Create(new Regex(expression, RegexOptions.IgnoreCase), creator));
                }
                else
                {
                    ContentTypeHandlerFunctions.TryAdd(contentType, creator);
                }
            }
        }

        private static bool TypeIsContentTypeHandler(Type type)
        {
            return (!type.IsAbstract) && (!type.IsInterface) && typeof (IContentTypeHandler).IsAssignableFrom(type);
        }
    }
}