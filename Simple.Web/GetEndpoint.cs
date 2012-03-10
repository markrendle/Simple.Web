namespace Simple.Web
{
    public abstract class GetEndpoint<TResponse> : IEndpoint
    {
        public object Run()
        {
            return Get();
        }

        protected abstract TResponse Get();
    }
}