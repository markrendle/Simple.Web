namespace Simple.Web.MediaTypeHandling
{
    using System.Collections.Generic;
    using System.Linq;
    using Simple.Web.Links;

    /// <summary>
    /// Default implementation of the <see cref="IContent"/> interface.
    /// </summary>
    class Content : IContent
    {
        private readonly object _handler;
        private readonly object _model;
        private readonly string _viewPath;

        internal Content(object handler, object model) : this(handler, model, null)
        {
        }

        internal Content(object handler, object model, string viewPath)
        {
            _handler = handler;
            _model = model;
            _viewPath = viewPath;
        }

        /// <summary>
        /// Gets the handler which generated the model.
        /// </summary>
        public object Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public object Model
        {
            get { return _model; }
        }

        /// <summary>
        /// Gets the variables from the handler.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Variables
        {
            get
            {
                return
                    _handler.GetType().GetProperties().Where(p => p.CanRead).Select(
                        p => new KeyValuePair<string, object>(p.Name, p.GetValue(_handler, null)));
            }
        }

        /// <summary>
        /// Gets the links which are valid for the model type, based on the <see cref="LinksFromAttribute"/> on handlers.
        /// </summary>
        public IEnumerable<Link> Links { get { return LinkHelper.GetLinksForModel(_model); } }
    }
}