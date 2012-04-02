namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    abstract class EndpointRunner : IEndpointRunner
    {
        private static readonly Dictionary<Type, Func<object,EndpointRunner>> Builders = new Dictionary<Type, Func<object, EndpointRunner>>
                                                                                             {
                                                                                                 { typeof(IGet), o => new GetRunner((IGet)o) },
                                                                                                 { typeof(IPost), o => new PostRunner((IPost)o) },
                                                                                             };

        private readonly object _endpoint;

        protected EndpointRunner(object endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException("endpoint");
            _endpoint = endpoint;
        }

        public object Endpoint
        {
            get { return _endpoint; }
        }

        public abstract Status Run();
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

        public static EndpointRunner Create<TMethod>(object endpoint)
        {
            return Builders[typeof (TMethod)](endpoint);
        }
    }
}