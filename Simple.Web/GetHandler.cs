namespace Simple.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    class GetHandler : HandlerBase
    {
        private static readonly object RoutingTableLock = new object();
        private static volatile bool _routingTableInitialized;
        private static RoutingTable _getRoutingTable;

        internal static void HandleRequest(HttpContext context)
        {
            IContentTypeHandler contentTypeHandler;
            try
            {
                contentTypeHandler = ContentTypeHandlerTable.GetContentTypeHandler(context.Request.AcceptTypes);
            }
            catch (UnsupportedMediaTypeException)
            {
                context.Response.StatusCode = 415;
                context.Response.Close();
                return;
            }

            Type endpointType;
            var variables = GetEndpointType(context, out endpointType);
            if (endpointType != null)
            {
                var endpoint = EndpointFactory.Instance.GetEndpoint(endpointType, variables);
                if (endpoint != null)
                {
                    var output = endpoint.Run();
                    using (var writer = new StreamWriter(context.Response.OutputStream))
                    {
                        contentTypeHandler.Write(output, writer);
                        context.Response.Flush();
                        context.Response.Close();
                    }
                }
            }
        }

        private static IDictionary<string, string> GetEndpointType(HttpContext context, out Type endpointType)
        {
            BuildGetRoutingTable();

            IDictionary<string, string> variables;
            endpointType = _getRoutingTable.Get(context.Request.Url.AbsolutePath, out variables);

            foreach (var key in context.Request.QueryString.AllKeys)
            {
                variables.Add(key, context.Request.QueryString[key]);
            }
            return variables;
        }

        private static void BuildGetRoutingTable()
        {
            if (!_routingTableInitialized)
            {
                lock (RoutingTableLock)
                {
                    if (!_routingTableInitialized)
                    {
                        _getRoutingTable = new RoutingTableBuilder(typeof(GetEndpoint<>)).BuildRoutingTable();
                        _routingTableInitialized = true;
                    }
                }
            }
        }
    }

    internal class HandlerBase
    {
        protected static readonly ContentTypeHandlerTable ContentTypeHandlerTable = new ContentTypeHandlerTable();
    }

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

    public class UnsupportedMediaTypeException : Exception
    {
        private readonly string[] _contentTypes;

        public UnsupportedMediaTypeException(string contentType)
        {
            _contentTypes = new[] {contentType};
        }
        public UnsupportedMediaTypeException(string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        public string[] ContentTypes
        {
            get { return _contentTypes; }
        }
    }

    public interface IContentTypeHandler
    {
        object Read(StreamReader streamReader);
        void Write(object obj, StreamWriter streamWriter);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContentTypesAttribute : Attribute
    {
        private readonly string[] _contentTypes;

        // This is a positional argument
        public ContentTypesAttribute(params string[] contentTypes)
        {
            _contentTypes = contentTypes;
        }

        public string[] ContentTypes
        {
            get { return _contentTypes; }
        }
    }
}