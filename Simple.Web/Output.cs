namespace Simple.Web
{
    using System.Collections.Generic;
    using System.Linq;

    class Content : IContent
    {
        private readonly EndpointRunner _runner;

        internal Content(EndpointRunner runner)
        {
            _runner = runner;
        }

        public object Model
        {
            get { return _runner.HasOutput ? _runner.Output : null; }
        }

        public IEnumerable<KeyValuePair<string, object>> Variables
        {
            get
            {
                return
                    _runner.Endpoint.GetType().GetProperties().Where(p => p.CanRead).Select(
                        p => new KeyValuePair<string, object>(p.Name, p.GetValue(_runner.Endpoint, null)));
            }
        }

        public string ViewPath
        {
            get
            {
                var isv = _runner.Endpoint as ISpecifyView;
                if (isv != null) return isv.ViewPath;
                return null;
            }
        }
    }
}