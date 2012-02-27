namespace Simple.Web
{
    public abstract class PostEndpoint<TModel> : PostEndpoint
    {
        internal TModel _model;
        protected TModel Model { get { return _model; } }
    }

    public abstract class PostEndpoint
    {
        internal PostEndpoint()
        {
        }

        protected internal abstract string UriTemplate { get; }
        protected internal abstract object Run();
    }
}