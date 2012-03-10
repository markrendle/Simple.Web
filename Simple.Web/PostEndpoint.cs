namespace Simple.Web
{
    public abstract class PostEndpoint<TRequest,TResponse> : IEndpoint
    {
        public object Run()
        {
            return Get();
        }

        public TRequest Model { get; set; }
        protected abstract TResponse Get();
    }
}