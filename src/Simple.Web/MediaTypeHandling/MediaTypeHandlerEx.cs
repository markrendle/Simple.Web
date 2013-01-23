namespace Simple.Web.MediaTypeHandling
{
    using System.Collections.Generic;
    using System.Linq;

    static class MediaTypeHandlerEx
    {
        /// <summary>
        /// Gets the first content-type match for the handler from a list of accepted types
        /// </summary>
        /// <param name="mediaTypeHandler">The content type handler.</param>
        /// <param name="acceptedTypes">The accepted types.</param>
        /// <returns>The MIME content type, or <c>null</c> if no match is found.</returns>
        public static string GetContentType(this IMediaTypeHandler mediaTypeHandler, IList<string> acceptedTypes)
        {
            return MediaTypesAttribute.Get(mediaTypeHandler.GetType()).SelectMany(cta => cta.ContentTypes)
                .FirstOrDefault(acceptedTypes.Contains);
        }
    }
}