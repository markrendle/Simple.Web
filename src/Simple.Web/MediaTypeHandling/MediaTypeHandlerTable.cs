namespace Simple.Web.MediaTypeHandling
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Simple.Web.Helpers;

    internal class MediaTypeHandlerTable
    {
        private static readonly object InitLock = new object();

        private static readonly ConcurrentDictionary<string, Func<IMediaTypeHandler>> MediaTypeHandlerFunctions =
            new ConcurrentDictionary<string, Func<IMediaTypeHandler>>();

        private static readonly HashSet<string> UnsupportedMediaTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly List<Tuple<Regex, Func<IMediaTypeHandler>>> WildcardMediaTypeHandlerFunctions =
            new List<Tuple<Regex, Func<IMediaTypeHandler>>>();

        private static bool _initialized;

        public IMediaTypeHandler GetMediaTypeHandler(string mediaType)
        {
            EnsureTableIsPopulated();

            var handler = GetMediaTypeHandlerImpl(mediaType);

            if (handler == null)
            {
                throw new UnsupportedMediaTypeException(mediaType);
            }

            return handler;
        }

        public IMediaTypeHandler GetMediaTypeHandler(IList<string> mediaTypes, out string matchedType)
        {
            if (mediaTypes == null)
            {
                throw new ArgumentNullException("mediaTypes");
            }
            EnsureTableIsPopulated();

            for (int i = 0; i < mediaTypes.Count; i++)
            {
                var handler = GetMediaTypeHandlerImpl(mediaTypes[i]);
                if (handler != null)
                {
                    matchedType = mediaTypes[i];
                    return handler;
                }
            }

            throw new UnsupportedMediaTypeException(mediaTypes);
        }

        private static void AddContentTypeHandler(Type exportedType)
        {
            var mediaTypes =
                Attribute.GetCustomAttributes(exportedType, typeof(MediaTypesAttribute))
                         .Cast<MediaTypesAttribute>()
                         .SelectMany(mediaTypesAttribute => mediaTypesAttribute.ContentTypes);

            Func<IMediaTypeHandler> creator = () => Activator.CreateInstance(exportedType) as IMediaTypeHandler;
            foreach (var mediaType in mediaTypes)
            {
                if (mediaType.Contains("*"))
                {
                    var expression = Regex.Escape(mediaType).Replace(@"\*", ".*?");
                    WildcardMediaTypeHandlerFunctions.Add(Tuple.Create(new Regex(expression, RegexOptions.IgnoreCase), creator));
                }
                else
                {
                    MediaTypeHandlerFunctions.TryAdd(mediaType, creator);
                }
            }
        }

        private static void EnsureTableIsPopulated()
        {
            if (!_initialized)
            {
                lock (InitLock)
                {
                    if (!_initialized)
                    {
                        PopulateContentTypeHandlerFunctions();
                        _initialized = true;
                    }
                }
            }
        }

        private static IMediaTypeHandler GetMediaTypeHandlerImpl(string mediaType)
        {
            int semiColon = mediaType.IndexOf(';');
            if (semiColon > -1)
            {
                mediaType = mediaType.Substring(0, semiColon);
            }

            if (UnsupportedMediaTypes.Contains(mediaType))
            {
                return null;
            }

            Func<IMediaTypeHandler> func;
            if (MediaTypeHandlerFunctions.TryGetValue(mediaType, out func))
            {
                return func();
            }

            var wildcard = WildcardMediaTypeHandlerFunctions.FirstOrDefault(t => t.Item1.IsMatch(mediaType));
            if (wildcard != null)
            {
                // Cache the specific content type for faster resolution next time.
                MediaTypeHandlerFunctions.TryAdd(mediaType, wildcard.Item2);
                return wildcard.Item2();
            }

            if (mediaType.StartsWith("*/*", StringComparison.OrdinalIgnoreCase))
            {
                return SimpleWeb.Configuration.DefaultMediaTypeHandler;
            }

            UnsupportedMediaTypes.Add(mediaType);
            return null;
        }

        private static void PopulateContentTypeHandlerFunctions()
        {
            foreach (var exportedType in ExportedTypeHelper.FromCurrentAppDomain(TypeIsContentTypeHandler))
            {
                AddContentTypeHandler(exportedType);
            }

            AddContentTypeHandler(typeof(FormDeserializer));
        }

        private static bool TypeIsContentTypeHandler(Type type)
        {
            return (!type.IsAbstract) && (!type.IsInterface) && typeof(IMediaTypeHandler).IsAssignableFrom(type);
        }
    }
}