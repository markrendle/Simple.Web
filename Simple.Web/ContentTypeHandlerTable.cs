namespace Simple.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class ContentTypeHandlerTable
    {
        private static readonly Dictionary<string, Func<IContentTypeHandler>> ContentTypeHandlerFunctions;
        private static readonly ICollection AsCollection;

        static ContentTypeHandlerTable()
        {
            AsCollection = ContentTypeHandlerFunctions = new Dictionary<string, Func<IContentTypeHandler>>();
        }

        public IContentTypeHandler GetContentTypeHandler(string contentType)
        {
            EnsureTableIsPopulated();

            Func<IContentTypeHandler> func;
            if (ContentTypeHandlerFunctions.TryGetValue(contentType, out func))
            {
                return func();
            }

            throw new UnsupportedMediaTypeException(contentType);
        }

        public IContentTypeHandler GetContentTypeHandler(string[] contentTypes)
        {
            EnsureTableIsPopulated();

            Func<IContentTypeHandler> func = null;
            if (contentTypes.Any(ct => ContentTypeHandlerFunctions.TryGetValue(ct, out func)))
            {
                return func();
            }

            throw new UnsupportedMediaTypeException(contentTypes);
        }

        private static void EnsureTableIsPopulated()
        {
            if ((!AsCollection.IsSynchronized) || ContentTypeHandlerFunctions.Count == 0)
            {
                lock (AsCollection.SyncRoot)
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
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
            {
                var postEndpointTypes = assembly.GetExportedTypes().Where(TypeIsContentTypeHandler).ToList();

                foreach (var exportedType in postEndpointTypes)
                {
                    AddContentTypeHandler(exportedType);
                }
            }
        }

        private static void AddContentTypeHandler(Type exportedType)
        {
            var contentTypes = Attribute.GetCustomAttributes(exportedType, typeof (ContentTypesAttribute))
                .Cast<ContentTypesAttribute>()
                .SelectMany(contentTypesAttribute => contentTypesAttribute.ContentTypes);

            foreach (var contentType in contentTypes)
            {
                ContentTypeHandlerFunctions.Add(contentType,
                                                () => Activator.CreateInstance(exportedType) as IContentTypeHandler);
            }
        }

        private static bool TypeIsContentTypeHandler(Type type)
        {
            return (!type.IsAbstract) && typeof (IContentTypeHandler).IsAssignableFrom(type);
        }
    }
}