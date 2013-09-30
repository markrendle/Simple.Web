namespace Simple.Web.Links
{
    using System;

    /// <summary>
    /// Represents a link, to be sent as part of a resource in a RESTful API.
    /// </summary>
    public class Link
    {
        private readonly Type _handlerType;
        private readonly string _href;
        private readonly string _rel;
        private readonly string _title;
        private string _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="handlerType">Type of the handler the link points to.</param>
        /// <param name="href">The href: the URI pointing to the resource/handler.</param>
        /// <param name="rel">The rel: the relationship of the link to the resource it is included with.</param>
        /// <param name="type">The type: the Content-Type returned by the link.</param>
        /// <param name="title">The title: a human-readable name for the link.</param>
        public Link(Type handlerType, string href, string rel, string type, string title)
        {
            _handlerType = handlerType;
            _href = href;
            _rel = rel;
            _type = type;
            _title = title;
        }

        /// <summary>
        /// Gets the href: the URI pointing to the resource/handler.
        /// </summary>
        public string Href
        {
            get { return _href; }
        }

        /// <summary>
        /// Gets the rel: the relationship of the link to the resource it is included with.
        /// </summary>
        public string Rel
        {
            get { return _rel; }
        }

        /// <summary>
        /// Gets the title: a human-readable name for the link.
        /// </summary>
        public string Title
        {
            get { return _title; }
        }

        /// <summary>
        /// Gets the type: the Content-Type returned by the link.
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets the type of the handler.
        /// </summary>
        /// <returns>The type of the handler.</returns>
        public Type GetHandlerType()
        {
            return _handlerType;
        }
    }
}