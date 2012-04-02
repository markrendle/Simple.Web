namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    abstract class AsyncEndpointRunner : IEndpointRunner
    {
        private static readonly Dictionary<Type, Func<object,AsyncEndpointRunner>> Builders = new Dictionary<Type, Func<object, AsyncEndpointRunner>>
                                                                                                  {
                                                                                                      { typeof(IGetAsync), o => new GetAsyncRunner((IGetAsync)o) },
                                                                                                      { typeof(IPostAsync), o => new PostAsyncRunner((IPostAsync)o) },
                                                                                                  };

        private readonly object _endpoint;

        protected AsyncEndpointRunner(object endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException("endpoint");
            _endpoint = endpoint;
        }

        public object Endpoint
        {
            get { return _endpoint; }
        }

        public abstract Task<Status> Run();
        public bool HasInput { get { return _endpoint.GetType().GetProperty("Input") != null; } }
        public bool HasOutput { get { return _endpoint.GetType().GetProperty("Output") != null; } }
        public object Input { set { _endpoint.GetType().GetProperty("Input").SetValue(_endpoint, value, null);} }
        public object Output { get { return _endpoint.GetType().GetProperty("Output").GetValue(_endpoint, null); } }

        public Type InputType
        {
            get
            {
                var iinput = _endpoint.GetType().GetInterface(typeof(IInput<>).Name);
                if (iinput != null)
                {
                    return iinput.GetGenericArguments()[0];
                }
                return null;
            }
        }

        public virtual void BeforeRun(IContext context, ContentTypeHandlerTable contentTypeHandlerTable)
        {
        }

        public static AsyncEndpointRunner Create<TMethod>(object endpoint)
        {
            return Builders[typeof (TMethod)](endpoint);
        }
    }
}