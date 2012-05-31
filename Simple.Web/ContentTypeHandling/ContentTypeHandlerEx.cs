namespace Simple.Web.ContentTypeHandling
{
    using System.Collections.Generic;
    using System.Linq;

    static class ContentTypeHandlerEx
    {
        /// <summary>
        /// Gets the first content-type match for the handler from a list of accepted types
        /// </summary>
        /// <param name="contentTypeHandler">The content type handler.</param>
        /// <param name="acceptedTypes">The accepted types.</param>
        /// <returns>The MIME content type, or <c>null</c> if no match is found.</returns>
        public static string GetContentType(this IContentTypeHandler contentTypeHandler, IList<string> acceptedTypes)
        {
            return ContentTypesAttribute.Get(contentTypeHandler.GetType()).SelectMany(cta => cta.ContentTypes)
                .FirstOrDefault(acceptedTypes.Contains);
        }
    }
}