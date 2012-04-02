namespace Simple.Web
{
    using System.Threading.Tasks;

    sealed class GetAsyncRunner : AsyncEndpointRunner
    {
        private readonly IGetAsync _get;

        public GetAsyncRunner(IGetAsync get) : base(get)
        {
            _get = get;
        }

        public override Task<Status> Run()
        {
            return _get.Get();
        }
    }
}