namespace Simple.Web
{
    public abstract class GetEndpoint<TResponse> : IEndpoint
    {
        public Status Run()
        {
            return Get();
        }

        public string UriTemplate
        {
            get { throw new System.NotImplementedException(); }
        }

        public TResponse Output { get; protected set; }

        object IEndpoint.Output
        {
            get { return Output; }
        }

        protected abstract Status Get();
    }
}