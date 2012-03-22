namespace Simple.Web
{
    sealed class GetRunner : EndpointRunner
    {
        private readonly IGet _get;

        public GetRunner(IGet get) : base(get)
        {
            _get = get;
        }

        public override Status Run()
        {
            return _get.Get();
        }
    }
}