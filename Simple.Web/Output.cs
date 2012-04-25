namespace Simple.Web
{
    using System.Collections.Generic;
    using System.Linq;

    class Content : IContent
    {
        private readonly object _endpoint;
        private readonly object _model;
        private readonly string _viewPath;

        internal Content(object endpoint, object model) : this(endpoint, model, null)
        {
        }

        internal Content(object endpoint, object model, string viewPath)
        {
            _endpoint = endpoint;
            _model = model;
            _viewPath = viewPath;
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
                    _endpoint.GetType().GetProperties().Where(p => p.CanRead).Select(
                        p => new KeyValuePair<string, object>(p.Name, p.GetValue(_endpoint, null)));
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