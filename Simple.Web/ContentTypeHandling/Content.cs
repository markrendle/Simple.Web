namespace Simple.Web.ContentTypeHandling
{
    using System.Collections.Generic;
    using System.Linq;

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

        public object Handler
        {
            get { return _handler; }
        }

        public object Model
        {
            get { return _model; }
        }

        public IEnumerable<KeyValuePair<string, object>> Variables
        {
            get
            {
                return
                    _handler.GetType().GetProperties().Where(p => p.CanRead).Select(
                        p => new KeyValuePair<string, object>(p.Name, p.GetValue(_handler, null)));
            }
        }

        public string ViewPath
        {
            get
            {
                return _viewPath;
            }
        }
    }
}