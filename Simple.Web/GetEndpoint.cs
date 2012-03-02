namespace Simple.Web
{
    public abstract class GetEndpoint<TResponse> : IEndpoint
    {
        public abstract string UriTemplate { get; }
        public object Run()
        {
            return Get();
        }

        protected abstract TResponse Get();
    }
}