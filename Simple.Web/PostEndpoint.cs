namespace Simple.Web
{
    public abstract class PostEndpoint<TRequest,TResponse> : IEndpoint
    {
        public abstract string UriTemplate { get; }
        public object Run()
        {
            return Get();
        }

        public TRequest Model { get; set; }
        protected abstract TResponse Get();
    }
}