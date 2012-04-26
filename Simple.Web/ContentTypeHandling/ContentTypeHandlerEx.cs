namespace Simple.Web.ContentTypeHandling
{
    using System.Collections.Generic;
    using System.Linq;

    static class ContentTypeHandlerEx
    {
        public static string GetContentType(this IContentTypeHandler contentTypeHandler, IList<string> acceptedTypes)
        {
            return ContentTypesAttribute.Get(contentTypeHandler.GetType()).SelectMany(cta => cta.ContentTypes)
                .FirstOrDefault(acceptedTypes.Contains);
        }
    }
}