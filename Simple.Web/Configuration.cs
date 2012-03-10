namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Configuration
    {
        private readonly Dictionary<string, Func<IContentTypeHandler>> _handlers =
            new Dictionary<string, Func<IContentTypeHandler>>();

        protected void AddContentTypeHandler(Func<IContentTypeHandler> handler, string contentType)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            if (handler == null) throw new ArgumentNullException("handler");
            _handlers.Add(contentType, handler);
        }

        protected void AddContentTypeHandler(Func<IContentTypeHandler> handler, params string[] contentTypes)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            foreach (var contentType in contentTypes.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                _handlers.Add(contentType, handler);
            }
        }

        internal IContentTypeHandler GetContentTypeHandler(string contentType)
        {
            if (_handlers.ContainsKey(contentType))
            {
                return _handlers[contentType]();
            }

            throw new UnsupportedMediaTypeException(contentType);
        }

        internal IContentTypeHandler GetContentTypeHandler(string[] contentTypes)
        {
            Func<IContentTypeHandler> contentTypeHandler = null;

            var foundType = contentTypes.FirstOrDefault(ct => _handlers.TryGetValue(ct, out contentTypeHandler));

            if (contentTypeHandler != null)
            {
                return contentTypeHandler();
            }

            throw new UnsupportedMediaTypeException(contentTypes.ToArray());
        }

        protected abstract void Configure();
    }
}