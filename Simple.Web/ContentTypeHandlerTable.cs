namespace Simple.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class ContentTypeHandlerTable
    {
        private static readonly Dictionary<string, Func<IContentTypeHandler>> ContentTypeHandlerFunctions =
            new Dictionary<string, Func<IContentTypeHandler>>();

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

        public IContentTypeHandler GetContentTypeHandler(IList<string> contentTypes)
        {
            EnsureTableIsPopulated();

            for (int i = 0; i < contentTypes.Count; i++)
            {
                Func<IContentTypeHandler> func;
                if (ContentTypeHandlerFunctions.TryGetValue(contentTypes[i], out func))
                {
                    return func();
                }
            }

            throw new UnsupportedMediaTypeException(contentTypes);
        }

        private static void EnsureTableIsPopulated()
        {
            if (ContentTypeHandlerFunctions.Count == 0)
            {
                lock (((ICollection) ContentTypeHandlerFunctions).SyncRoot)
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

        internal static void SetContentTypeHandlers(params Type[] types)
        {
            foreach (var type in types)
            {
                AddContentTypeHandler(type);
            }

            AddContentTypeHandler(typeof(FormDeserializer));
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

    static class ExportedTypeHelper
    {
        public static IEnumerable<Type> FromCurrentAppDomain(Func<Type,bool> predicate)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(assembly =>
                    assembly.GetExportedTypes().Where(predicate).ToList())
                .SelectMany(exportedTypes => exportedTypes);
        }
    }
}