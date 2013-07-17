using System;

namespace Simple.Web.MediaTypeHandling
{
    using System.Collections.Generic;
    using System.Linq;

    static class MediaTypeHandlerEx
    {
        private static readonly IDictionary<Type, HashSet<string>> Cache = new Dictionary<Type, HashSet<string>>();
        private static readonly object Sync = new object();

        /// <summary>
        /// Gets the first content-type match for the handler from a list of accepted types
        /// </summary>
        /// <param name="mediaTypeHandler">The content type handler.</param>
        /// <param name="acceptedTypes">The accepted types.</param>
        /// <returns>The MIME content type, or <c>null</c> if no match is found.</returns>
        public static string GetContentType(this IMediaTypeHandler mediaTypeHandler, IList<string> acceptedTypes)
        {
            HashSet<string> contentTypes;
            if (!Cache.TryGetValue(mediaTypeHandler.GetType(), out contentTypes))
            {
                lock (Sync)
                {
                    if (!Cache.TryGetValue(mediaTypeHandler.GetType(), out contentTypes))
                    {
                        contentTypes =
                            new HashSet<string>(
                                MediaTypesAttribute.Get(mediaTypeHandler.GetType()).SelectMany(cta => cta.ContentTypes),
                                StringComparer.OrdinalIgnoreCase);
                        Cache[mediaTypeHandler.GetType()] = contentTypes;
                    }
                }
            }
            return acceptedTypes.FirstOrDefault(x => ContainsMatchingContentType(contentTypes, x));
        }

        private static bool ContainsMatchingContentType(IEnumerable<string> supportedMediaTypes, string mediaType)
        {
            bool matched = false;
            foreach (var supportedMediaType in supportedMediaTypes)
            {
                if (IsWildCardMediaType(supportedMediaType))
                {
                    matched = MatchesWildCard(supportedMediaType, mediaType);
                }
                else
                {
                    matched = (supportedMediaType == mediaType);
                }

                if (matched)
                {
                    break;
                }
            }
            return matched;
        }

        private static bool IsWildCardMediaType(string mediaType)
        {
            return mediaType.Contains("*");
        }

        private static bool MatchesWildCard(string wildcard, string mediaType)
        {
            var parts = wildcard.Split('*');
            return (mediaType.StartsWith(parts[0]) && mediaType.EndsWith(parts[1]));
        }
    }
}